using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

public class ProgressBarCon : ProgressBar
{

    private void Update()
    {
        if(isActive)
        {
            Progress();
        }
        
    }

    new void Progress()
    {
        progressTime += Time.deltaTime;
        progressRatio = progressTime / needTime;
        float changedSize = progressRatio * maxWidth;
        if (0 < progressRatio && progressRatio < 1.0)
        {
            changePanelSize(ref panelTransform, changedSize);
        }
        if (progressRatio >= 1)
        {
            changePanelSize(ref panelTransform, maxWidth);
            CompleteTask();
        }
    }

    new void CompleteTask()
    {
        Destroy(this.gameObject);
    }

    
}
