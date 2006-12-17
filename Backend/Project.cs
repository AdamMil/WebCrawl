using System;

namespace WebCrawl.Backend
{

public class Project
{
  public bool PurgeOldFiles
  {
    get { return purgeOld; }
    set { purgeOld = value; }
  }
  
  public bool RedownloadErasedFiles
  {
    get { return downloadErased; }
    set { downloadErased = value; }
  }

  bool purgeOld = true, downloadErased = true;
}

} // namespace WebCrawl.Backend