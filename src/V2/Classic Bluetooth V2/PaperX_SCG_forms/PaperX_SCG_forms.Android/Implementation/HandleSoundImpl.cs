using Android.App;
using Android.Media;
using PaperX_SCG_forms.Interface;

namespace PaperX_SCG_forms.Droid.Implementation
{
    public class HandleSoundImpl : IHandleSound
    {
        public void PlaySound()
        {
            var player = new MediaPlayer();
            var file = Application.Context.Assets.OpenFd("sound.wav");
            player.SetDataSource(file.FileDescriptor, file.StartOffset, file.Length);
            player.Prepare();
            player.Start();
            player.Completion += (s, e) =>
            {
                player.Release();
                player.Dispose();
            };
        }
    }
}