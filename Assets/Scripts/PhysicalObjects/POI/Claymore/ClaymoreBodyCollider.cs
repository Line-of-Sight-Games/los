using UnityEngine;

public class ClaymoreBodyCollider : BaseBodyCollider
{
    public Claymore linkedClaymore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        linkedBody = linkedClaymore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Claymore LinkedClaymore 
    { 
        get { return linkedClaymore; } 
        set { linkedClaymore = value; } 
    }
}
