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

		private Dictionary<string, ClientConnection> connections = new Dictionary<string, ClientConnection>();

		public void AddConnection(string secret, string reqKeyword, HttpResponse response)
		{
			// 중복된 키로 요청이 들어오면 이전 커넥션 캔슬 (자동으로 Dispose 될 것)
			if (connections.TryGetValue(secret, out var connection))
			{
				connection.Cancel();
			}

			// 새로운 커넥션 등록 (이전 커넥션은 캔슬되었으므로, 그냥 덮어씀)
			connections[secret] = new ClientConnection(secret, reqKeyword, response, () =>
			{
				connections.Remove(secret);
			});
		}

		public bool IsAliveConnection(string secret)
		{
			return connections.ContainsKey(secret);
		}
	}
}
