using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BulletModel : MonoBehaviour {

	static Mesh BulletMesh {
		// 値の取得用
		get {
			// 最初だけ 
			if (_BulletMesh == null) {
				// Cubeを作成
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

				// Cubeの共有メッシュを取得
				_BulletMesh = Instantiate(go.GetComponent<MeshFilter>().sharedMesh);

				// Cubeの頂点情報(24個)を配列に取得
				Vector3[] vertices = _BulletMesh.vertices;

				// Cubeの頂点情報を変更
				var vList = vertices.Select( v => 
					{ v.x *= 0.7f * (0.5f - v.z);
                      v.y *= 0.7f * (0.5f - v.z);
                      v.z *= 5.0f;
					  return v;
					} ).ToList();

				// 新しい頂点情報を適用
				_BulletMesh.SetVertices(vList);

				// Meshの名前
				_BulletMesh.name = "BulletMesh";

				_BulletMesh.hideFlags = HideFlags.HideAndDontSave;

				Destroy(go);
			}

			// いま作成したMeshを返す
			return _BulletMesh;
		}
	}
	static Mesh _BulletMesh;
	public Color col = Color.yellow;

	// Use this for initialization
	void Start () {
		// Meshをアド
		gameObject.AddComponent<MeshFilter>().sharedMesh = BulletMesh;
		float tmp = Random.Range(1.0f, 2.0f);
		gameObject.transform.localScale = new Vector3(tmp, tmp, tmp);

		var mat = gameObject.AddComponent<MeshRenderer>().material;
		mat.color = this.col;
		var trail = gameObject.AddComponent<TrailRenderer>();

		trail.time = tmp*1.0f; // 軌跡が消えるまでの時間
		trail.startWidth = tmp*0.7f;
		trail.endWidth = 0f;
		trail.material = mat;
	}
}
