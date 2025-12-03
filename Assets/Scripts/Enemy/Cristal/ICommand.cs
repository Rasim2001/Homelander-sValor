using System.Threading;
using Cysharp.Threading.Tasks;

namespace Enemy.Cristal
{
    public interface ICommand
    {
        UniTask Execute(CancellationToken cancellationToken);
    }
}