namespace LabProject.Audio.IO;

using System.IO;

public class TempFileStream(string path, FileMode mode)
    : FileStream(path, mode)
{
    private readonly string _path = path;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        File.Delete(this._path);
    }
}