using UnityEngine;

namespace Infastructure.Services.QuitGameApplicationService
{
    public class QuitGameService : IQuitGameService
    {
        private const string GoogleFormURL =
            "https://docs.google.com/presentation/d/1zr8H-iInoHpNdxcrpx-iSJEIUBFjTvoKVJw4R7QFOEA/edit?slide=id.g358d2026a78_2_105#slide=id.g358d2026a78_2_105";

        public void QuitGame()
        {
            //Application.OpenURL(GoogleFormURL);
            Application.Quit();
        }
    }
}