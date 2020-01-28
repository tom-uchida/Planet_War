using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticuleController : MonoBehaviour {
	public Texture2D reticule;	//　カーソルに使用するテクスチャ
 
	void Start () {
		//　カーソルを自前のカーソルに変更
 		Cursor.SetCursor(this.reticule, 
 						 new Vector2(this.reticule.width/2, this.reticule.height/2), 
 						 CursorMode.Auto);
	}
}
