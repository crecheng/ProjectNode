using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace AOT
{
    public class AOTLaunch: MonoBehaviour
    {
        public static AOTLaunch Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");

            Application.backgroundLoadingPriority = ThreadPriority.High;
        }

        private async void Start()
        {
            if (Application.isEditor)
            {
                await this.ProcessingEditorAsync();
            }
        }
        
        private async Task ProcessingEditorAsync()
        {
            AOTLaunch.Instance.Log("ProcessingEditor");

            InEditor:
            try
            {
                var asm = Assembly.Load("Assembly-CSharp");
                var gameStart = asm.GetType("GameLauncher");
                var launcherMethod = gameStart.GetMethod("LaunchGame", BindingFlags.NonPublic | BindingFlags.Static);
                launcherMethod.Invoke(null, new object[] { "" });
            }
            catch (Exception e)
            {
                await AOTLaunch.Instance.LogExceptionAsync(e.ToString());
                //goto InEditor;
            }
        }

        private void Log(string msg)
        {
            Debug.Log(msg);
        }
        
        private async Task LogExceptionAsync(string msg)
        {
            Debug.LogError(msg);
            await UploadException(msg);
        }

        private async Task UploadException(string msg)
        {
            await Task.CompletedTask;
        }
    }
}