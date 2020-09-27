using System;

/// <summary>
/// GameCommandを提供する
/// </summary>
public interface IGameCommandProvider
{
    /// <summary>
    /// 入力されたGameCommandを購読します
    /// </summary>
    IObservable<GameCommand> ObservableGameCommand { get; }
}
