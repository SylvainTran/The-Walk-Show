using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoController : MonoBehaviour
{
    public GameObject evaNews1Video;
    public GameObject tokkingHeadVideo;
    public GameObject currentVideoPlaying;

    public void PlayVideo(int videoIndex)
    {
        if(currentVideoPlaying != null)
        {
            currentVideoPlaying.SetActive(false);
            currentVideoPlaying = null;
        }
        switch (videoIndex)
        {
            case 0:
                currentVideoPlaying = evaNews1Video;
                break;
            case 1:
                currentVideoPlaying = tokkingHeadVideo;
                break;
            default:
                currentVideoPlaying = evaNews1Video;
                break;
        }
        currentVideoPlaying.SetActive(true);
    }
}
