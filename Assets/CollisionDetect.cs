using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    private float lasttime = 0;

    public float interval = 0.5f;



    private void OnTriggerEnter(Collider other)
	{
		//print("ccc");
		if (other.tag == "Wall")
		{
            
            if((Time.time - lasttime)> interval)
			{
                UIMgr.GetInstance().ShowToast("ÒÑ¼ÝÊ»µ½±ß½ç");
                lasttime = Time.time;
            }
           
        }
	}

	private void on(Collision collision)
	{
		
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
