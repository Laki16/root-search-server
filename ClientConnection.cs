using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using ApiServer.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;

namespace ApiServer
{
	public class ClientConnection
	{
		/// <summary>
		/// 검색 및 클라이언트 전송 작업의 텀(ms)
		/// </summary>
		private const int TaskRunDelay = 5000;

		/// <summary>
		/// 이 클라이언트의 Request 시크릿 키
		/// </summary>
		private string secret;

		/// <summary>
		/// 검색 결과 큐
		/// </summary>
		private readonly BlockingCollection<SearchResultDTO> results;

		/// <summary>
		/// 검색할 키워드 큐
		/// </summary>
		private readonly BlockingCollection<string> searchTargets;

		/// <summary>
		/// 검색 완료한 검색어 목록
		/// </summary>
		private readonly HashSet<string> searchedKeywords;

		/// <summary>
		/// 커넥션 HttpResponse
		/// </summary>
		private readonly HttpResponse response;

		/// <summary>
		/// 커넥션이 캔슬되었는지 확인하기 위한 토큰
		/// </summary>
		private readonly CancellationTokenSource cancellationTokenSource;

		/// <summary>
		/// 전송 예약되어있는 검색 결과 사이즈
		/// </summary>
		private const int MaxPendingResultsCount = 10;

		/// <summary>
		/// 검색 예약되어있는 연관 키워드 수
		/// </summary>
		private const int MaxPendingAssociatedKeywordsCount = 5;

		/// <summary>
		/// 마지막으로 헬스체크 성공한 시간 (UTC)
		/// </summary>
		private DateTime lastHealthChecked;

		/// <summary>
		/// Cancel될 때 수행할 콜백
		/// </summary>
		private Action onCancel;

		public ClientConnection(string secret, string reqKeyword, HttpResponse response, Action onCancel)
		{
			this.secret = secret;

			this.response = response;

			this.results = new BlockingCollection<SearchResultDTO>(MaxPendingResultsCount);

			this.searchTargets = new BlockingCollection<string>(MaxPendingAssociatedKeywordsCount);
			this.searchTargets.TryAdd(reqKeyword);

			this.searchedKeywords = new HashSet<string>();

			this.cancellationTokenSource = new CancellationTokenSource();

			this.onCancel = onCancel;

			Task.Run(async () => await Run(this.cancellationTokenSource.Token));
		}

		private async Task Run(CancellationToken cancellationToken)
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var currentTime = DateTime.UtcNow;
					var healthChecked = false;

					healthChecked |= await TrySearch();

					healthChecked |= await TrySendDataToClient();

					// 비동기 작업 수행중에 헬스체크가 되었다면 따로 진행하지 않고 이번 텀을 넘김
					if (healthChecked)
					{
						lastHealthChecked = DateTime.UtcNow;
					}
					else if (DateTime.Compare(lastHealthChecked.AddSeconds(15), currentTime) < 0)
					{
						// need health check
						await response.Body.WriteAsync(Encoding.UTF8.GetBytes(":\n\n"));

						if (response.HttpContext.RequestAborted.IsCancellationRequested)
						{
							Cancel();
						}
						else
						{
							lastHealthChecked = currentTime;
						}
					}

					await Task.Delay(TaskRunDelay, cancellationToken).ConfigureAwait(false);
				}
			}
			catch (Exception e)
			{

			}
			finally
			{
				Dispose();
			}
		}

		public void Cancel()
		{
			cancellationTokenSource.Cancel();
		}

		private async Task<Boolean> TrySearch()
		{
			if (searchTargets.TryTake(out var keyword))
			{
				// 이미 검색한 키워드에 추가
				searchedKeywords.Add(keyword);

				var result = await WorkerManager.Instance.GetResult(keyword);

				results.TryAdd(new SearchResultDTO
				{
					KeyWord = keyword,
					Results = result.Results,
				});

				// 연관 검색어들을 검색 타겟에 설정
				foreach (var associated in result.AssociativeWords)
				{
					// 이미 검색된 키워드는 제외
					if (searchedKeywords.Contains(associated))
					{
						continue;
					}

					searchTargets.TryAdd(associated);
				}

				return response.HttpContext.RequestAborted.IsCancellationRequested;
			}

			return false;
		}

		private async Task<Boolean> TrySendDataToClient()
		{
			// TODO: threshold 지정? 네트워크 부하가 크면 고려해보자
			if (results.TryTake(out var result))
			{
				var builder = new StringBuilder();

				builder.Append($"data:{JsonConvert.SerializeObject(result)}\n\n");

				var message = Encoding.UTF8.GetBytes(builder.ToString());

				await response.Body.WriteAsync(message, 0, message.Length);

				return response.HttpContext.RequestAborted.IsCancellationRequested;
			}

			return false;
		}

		public void Dispose()
		{
			searchTargets.CompleteAdding();
			results.CompleteAdding();
		}
	}
}
