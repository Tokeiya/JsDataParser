using System;

namespace Parseq
{
	public interface IReply<out TToken, out T>
		: IEither<string, T>
	{
		TResult Case<TResult>(
			Func<ITokenStream<TToken>, string, TResult> failure,
			Func<ITokenStream<TToken>, T, TResult> success);
	}

	public partial class Reply
	{
		public static IReply<TToken, T> Success<TToken, T>(ITokenStream<TToken> restStream, T value)
		{
			return new SuccessImpl<TToken, T>(restStream, value);
		}

		public static IReply<TToken, T> Failure<TToken, T>(ITokenStream<TToken> restStream, string errorMessage)
		{
			return new FailureImpl<TToken, T>(restStream, errorMessage);
		}
	}

	public partial class Reply
	{
		private class SuccessImpl<TToken, T>
			: IReply<TToken, T>
		{
			private readonly ITokenStream<TToken> restStream;
			private readonly T value;

			public SuccessImpl(ITokenStream<TToken> restStream, T value)
			{
				this.restStream = restStream;
				this.value = value;
			}

			public TResult Case<TResult>(
				Func<ITokenStream<TToken>, string, TResult> failure,
				Func<ITokenStream<TToken>, T, TResult> success)
			{
				return success(restStream, value);
			}

			TResult IEither<string, T>.Case<TResult>(
				Func<string, TResult> left,
				Func<T, TResult> right)
			{
				return right(value);
			}
		}

		private class FailureImpl<TToken, T>
			: IReply<TToken, T>
		{
			private readonly string errorMessage;
			private readonly ITokenStream<TToken> restStream;

			public FailureImpl(ITokenStream<TToken> restStream, string errorMessage)
			{
				this.restStream = restStream;
				this.errorMessage = errorMessage;
			}

			public TResult Case<TResult>(
				Func<ITokenStream<TToken>, string, TResult> failure,
				Func<ITokenStream<TToken>, T, TResult> success)
			{
				return failure(restStream, errorMessage);
			}

			TResult IEither<string, T>.Case<TResult>(
				Func<string, TResult> left,
				Func<T, TResult> right)
			{
				return left(errorMessage);
			}
		}
	}
}