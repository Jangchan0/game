using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour 
{
    [SerializeField] float X_m_speed = 4.0f;
    [SerializeField] float Y_m_speed = 4.0f;
    [SerializeField] float m_rollForce = 6.0f;    
    [SerializeField] bool  m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] private bl_Joystick joystick;

    private Animator  m_animator;
    private Rigidbody2D m_body2d;
    // private Sensor_HeroKnight m_groundSensor;
    // private Sensor_HeroKnight m_wallSensorR1;
    // private Sensor_HeroKnight m_wallSensorR2;
    // private Sensor_HeroKnight m_wallSensorL1;
    // private Sensor_HeroKnight m_wallSensorL2;
    
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int  m_facingDirection = 1;
    private int  m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();

        // 만약 Sensor를 전혀 안 쓴다면 주석 처리 (Hierarchy에 자식 오브젝트가 없으면 NullReference 납니다.)
        // m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        // m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        // m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        // m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        // m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    void Update ()
    {
        // -- Attack Timer --
        m_timeSinceAttack += Time.deltaTime;

        // -- Rolling Timer --
        if (m_rolling)
        {
            m_rollCurrentTime += Time.deltaTime;
            if (m_rollCurrentTime > m_rollDuration)
                m_rolling = false;
        }

        // 항상 땅에 있다고 가정
        m_grounded = true;
        m_animator.SetBool("Grounded", m_grounded);

        // -- Joystick Input --
        float inputX = joystick.Horizontal;
        float inputY = joystick.Vertical;

        // -- Flip Sprite --
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // -- Movement --
        if (!m_rolling)
        {
            // x축: X_m_speed, y축: Y_m_speed
            m_body2d.linearVelocity = new Vector2(inputX * X_m_speed, inputY * Y_m_speed);
        }

        // 애니메이션 파라미터
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // -- Wall Slide (필요 없으면 주석)
        // if (m_wallSensorR1 != null && m_wallSensorR2 != null && m_wallSensorL1 != null && m_wallSensorL2 != null)
        // {
        //     m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) 
        //                     || (m_wallSensorL1.State() && m_wallSensorL2.State());
        // }
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // -- Key Inputs: Death, Hurt, Attack, Block, Roll --
        if (Input.GetKeyDown(KeyCode.E) && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !m_rolling)
        {
            m_animator.SetTrigger("Hurt");
        }
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3) m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f) m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;
        }
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            m_animator.SetBool("IdleBlock", false);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_rollCurrentTime = 0f;
            m_animator.SetTrigger("Roll");
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
        }
        // -- Run / Idle --
        else if (Mathf.Abs(inputX) > 0.01f || Mathf.Abs(inputY) > 0.01f)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1); // Run
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if(m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0); // Idle
        }
    }

    // 슬라이드 애니메이션 이벤트
    void AE_SlideDust()
    {
        if (m_slideDust != null)
        {
            Vector3 spawnPosition = transform.position;
            GameObject dust = Instantiate(m_slideDust, spawnPosition, transform.localRotation);
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}
