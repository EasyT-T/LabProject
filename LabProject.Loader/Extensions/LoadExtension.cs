namespace LabProject.Loader.Extensions;

using LabProject.Abstractions;
using LabProject.DI;

public static class LoadExtension
{
    extension(ILoad load)
    {
        public void LoadSafely(IExceptionHandler handler)
        {
            try
            {
                load.Load();
            }
            catch (Exception e)
            {
                handler.HandleException(e, $"Encountered an exception while loading the service \"{load.GetType().FullName}\"");
            }
        }
    }

    extension(IUnload unload)
    {
        public void UnloadSafely(IExceptionHandler handler)
        {
            try
            {
                unload.Unload();
            }
            catch (Exception e)
            {
                handler.HandleException(e, $"Encountered an exception while unloading the service \"{unload.GetType().FullName}\"");
            }
        }
    }
}