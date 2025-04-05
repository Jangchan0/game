using UnityEngine;

public class MonsterFollow : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 3.0f;
    // 용사 캐릭터의 크기에 맞춰서 멈출 거리를 Inspector에서 설정하거나, 동적으로 계산할 수 있음.
    // 여기서는 간단하게 Inspector에서 설정한 값으로 사용.
    public float stopDistance = 1.0f;

    void Update() {
        if (target == null) {
            return;
        }
        
        // 용사와의 방향 및 거리 계산 (z축은 0으로 고정)
        Vector3 direction = target.position - transform.position;
        direction.z = 0;
        
        // 용사와의 거리가 stopDistance보다 크면 이동
        if (direction.magnitude > stopDistance)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
        if (direction.magnitude <= stopDistance)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("AttackTrigger");
        }
        
        // 이동 방향에 따라 고블린이 용사를 바라보도록 좌우 뒤집기
        // direction.x가 양수면 용사가 오른쪽에 있으므로 고블린이 오른쪽(기본 방향)을 보도록 함.
        // direction.x가 음수면 왼쪽을 보도록 localScale의 x값을 음수로 설정.
        if (direction.x > 0) {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);  // 오른쪽 바라보기
            transform.localScale = scale;
        }
        else if (direction.x < 0) {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); // 왼쪽 바라보기
            transform.localScale = scale;
        }
    }
}
