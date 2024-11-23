using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public string cannonOwner; //⭐️誰のCannonかどうか

    public float speed = 5.0f; // Positionまで持っていく速さ
    private float rotateSpeed = 40.0f; // 回転の速さ
    float currentAngle; // Cannonのx角度を保持 ①

    public float bulletZ = -14.7f; //⭐️ Player用の球の角度

    public GameObject bulletPrefab; //bulletプレハブを入れておく。
    public float bulletSpeed = 1.0f; // bulletの速さ

    Vector3 targetPosition; // 目標Positionまでの距離
    Quaternion targetRotation; // 目標角度までの距離

    // Start is called before the first frame update
    void Start()
    {
        currentAngle = transform.localRotation.eulerAngles.x; // Cannonのx角度を保持 ②
    }

    // Grassからクリックした値を取得し、目標物設定。
    public void MoveToTarget(float grassX)
    {
        targetPosition = new Vector3(grassX, transform.position.y, transform.position.z); // Positionの位置設定
        targetRotation = Quaternion.Euler(-40.0f, transform.rotation.y, transform.rotation.z); // Positionの回転角度設定

        StartCoroutine(MoveCoroutine(targetPosition, targetRotation, bulletZ, speed)); // 移動用メソッドへ移動
    }

    // オブジェクト移動用メソッド
    private IEnumerator MoveCoroutine(Vector3 targetPosition, Quaternion targetRotation, float bulletZ, float speed)
    {
        //Grassの位置まで移動
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        
        //-20fまで回転
        while (currentAngle > -20f)
        {
            currentAngle -= Time.deltaTime * rotateSpeed;
            transform.localRotation = Quaternion.Euler(currentAngle, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
            yield return null;
        }

        // インスタンス生成し、球を打ち出す。
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(transform.position.x, 2.73f, bulletZ), Quaternion.identity);
        bullet.GetComponent<Bullet>().bulletOwner = this.cannonOwner; // 球のオブジェクトにどちらの球かの情報格納
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        //rb.AddForce((transform.forward + transform.up) * bulletSpeed, ForceMode.Impulse);
    }
}