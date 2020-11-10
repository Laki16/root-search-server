using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ApiServer.Models;
using Microsoft.AspNetCore.Http;

namespace ApiServer
{
	public class ResultManager
	{
		private static ResultManager _instance = new ResultManager();

		public static ResultManager Instance => _instance;

		private ConcurrentDictionary<string, ClientConnection> connections =
			new ConcurrentDictionary<string, ClientConnection>();

		/// <summary>
		/// 새로운 커넥션 등록
		/// </summary>
		/// <param name="secret">클라이언트 시크릿</param>
		/// <param name="reqKeyword">요청한 검색 키워드</param>
		/// <param name="response">요청한 action의 HttpResponse</param>
		/// <returns>커넥션이 정상적으로 등록되었다면 true</returns>
		public bool AddConnection(string secret, string reqKeyword, HttpResponse response)
		{
			// 중복된 키로 요청이 들어오면 이전 커넥션 캔슬 (자동으로 Dispose 될 것)
			if (connections.TryRemove(secret, out var connection))
			{
				connection.Cancel();
			}

			// 새로운 커넥션 생성
			connection = new ClientConnection(secret, reqKeyword, response, () =>
			{
				// 커넥션이 캔슬되었을 때 커넥션에서 자동으로 삭제되도록 콜백 등록
				connections.TryRemove(secret, out _);
			});

			// 새로운 커넥션 등록
			if (!connections.TryAdd(secret, connection))
			{
				// Duplicated Error! 중복에러 발생시 캔슬
				connection.Cancel();

				return false;
			}

			return true;
		}

		public bool IsAliveConnection(string secret)
		{
			return connections.ContainsKey(secret);
		}
	}
}
