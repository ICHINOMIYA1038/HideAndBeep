using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    public GameObject viewcamera;//"viewcamera"�Ƃ����ϐ����`

    void Update()
    {
        float mouse_x = Input.GetAxis("Mouse X");
        float mouse_y = Input.GetAxis("Mouse Y");
        transform.Rotate(0.0f, mouse_x, 0.0f);
        //����script�ɓo�^����Ă���I�u�W�F�N�g��X������-mouse_y�̒l���A��]����
        viewcamera.transform.Rotate(-mouse_y, 0.0f, 0.0f);
    }
}