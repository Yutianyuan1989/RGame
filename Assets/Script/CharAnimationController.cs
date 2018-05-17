using Engine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimationController : MonoBehaviour {

    public enum eDir
    {
        DOWN,
        LEFT,
        RIGHT,
        UP
    }

    public SpriteAnimData _SpriteAnimData;
    public SpriteRenderer TargetSpriteRenderer;
    public bool IsAnimated = true;
    public bool IsPingPongAnim = true; // set true for ping pong animation
    public float AnimSpeed = 9f; // frames per second
    public Vector2[] Pivot = null;
    private int AnimFrames;

    public eDir CurrentDir
    {
        get { return m_currentDir; }
        set
        {
            bool hasChanged = m_currentDir != value;
            m_currentDir = value;
            if (hasChanged)
            {
                //if (m_charsetType == eCharSetType.RPG_Maker_XP)
                //    TargetSpriteRenderer.sprite = m_spriteXpIdleFrames[(int)m_currentDir];
                //else
                TargetSpriteRenderer.sprite = GetCurrentSprite(m_currentDir);
            }
        }
    }
    private int m_internalFrame;
    private float m_curFrameTime;
    [SerializeField]
    private eDir m_currentDir = eDir.DOWN;
    [SerializeField]
    private List<Sprite> m_spriteFrames = new List<Sprite>();

    void Start() 
    {
        AnimFrames = _SpriteAnimData.DownSprites.Length;
    }

	// Update is called once per frame
	void Update () 
    {
        UpdateAnim(Time.deltaTime);
        InputKeyBoardControl();
	}

    private void UpdateAnim(float dt)
    {
        if (IsAnimated)
        {
            if (IsPingPongAnim && (m_internalFrame == 0 || m_internalFrame == (AnimFrames - 1)))
                m_curFrameTime += dt * AnimSpeed * 2f; // avoid stay twice of the time in the first and last frame of the animation
            else
                m_curFrameTime += dt * AnimSpeed;
            while (m_curFrameTime >= 1f)
            {
                m_curFrameTime -= 1f;
                ++m_internalFrame; 
                m_internalFrame %= AnimFrames;
            }
        }
        else
        {
            m_internalFrame = 0;
        }
        TargetSpriteRenderer.sprite = GetCurrentSprite(CurrentDir);

        this.transform.localPosition += Time.deltaTime * moveVector;
    }

    public Sprite GetCurrentSprite(eDir dir)
    {
        switch (dir)
        {
            case eDir.DOWN:
                return _SpriteAnimData.DownSprites[m_internalFrame];
                
            case eDir.UP:
                return _SpriteAnimData.UpSprites[m_internalFrame];
                
            case eDir.LEFT:
                return _SpriteAnimData.LeftSprites[m_internalFrame];
                
            case eDir.RIGHT:
                return _SpriteAnimData.RightSprites[m_internalFrame];
                
            default :
                return _SpriteAnimData.DownSprites[0];
                
        }

            
    }

    void InputKeyBoardControl()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) 
        {
            CurrentDir = eDir.UP;
            moveVector = Vector3.up;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            CurrentDir = eDir.DOWN;
            moveVector = Vector3.down;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            CurrentDir = eDir.LEFT;
            moveVector = Vector3.left;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            CurrentDir = eDir.RIGHT;
            moveVector = Vector3.right;
        }
    }

    void OnEnable()
    {
        EventEngine.Instance().AddEventListener((int)GameEventID.PLAYERMOVE, InputKeyBoardControl);
    }

    private float moveVectory = 1f;
    private Vector3 moveVector = Vector3.zero;

    private void InputKeyBoardControl(int nEventID, object param)
    {
        Vector2 vector = (Vector2)param ;      
        moveVector = (Vector3.up * vector.y + Vector3.right * vector.x) * moveVectory;
        FaceDirction();
    }

    void FaceDirction()
    {
        if (moveVector.y > 0.5f) 
        {
            CurrentDir = eDir.UP;
        }
        else if (moveVector.y < -0.5f)
        {
            CurrentDir = eDir.DOWN;
        }
        else if (moveVector.x < -0.5f)
        {
            CurrentDir = eDir.LEFT;
        }
        else if (moveVector.x > 0.5f)
        {
            CurrentDir = eDir.RIGHT;
        }
    }

    void OnDisable() 
    {
        EventEngine.Instance().RemoveAllEventListener((int)GameEventID.PLAYERMOVE);
    }

}
