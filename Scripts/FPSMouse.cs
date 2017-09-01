using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouse : MonoBehaviour {

	// Eixo
	public enum RotationAxis {MouseX, MouseY};
	public RotationAxis eixos = RotationAxis.MouseY;

	// Mouse

	private float sensAtualX = 1.5f;
	private float sensAtualY = 1.5f;
	private float sensX = 1.5f;
	private float sensY = 1.5f;

	private float sensMouse = 1.7f;

	// Rotacao
	
	private float rotacaoX, rotacaoY;
		//Controle de angulo
	private float minimoX = -360f;
	private float maximoX = 360f;
	private float minimoY = -60f;
	private float maximoY= 60f;

	// Bases
	private Quaternion rotacaoOriginal;

	// Use this for initialization
	void Start () {
		rotacaoOriginal = transform.rotation;
	}
	
	// Update is called once per frame	
	void LateUpdate(){
		ManipulaMouse();
	}

	float LimitarAngulos(float angulo, float min, float max){
		if(angulo < -360f){
			angulo += 360f;
		}

		if(angulo > 360f){
			angulo -= 360f;
		}

		return Mathf.Clamp(angulo, min, max);
	}

	void ManipulaMouse(){

		if(sensAtualX != sensMouse || sensAtualY != sensMouse){
			sensAtualX = sensAtualY = sensMouse;
		}

		sensX = sensAtualX;
		sensY = sensAtualY;
		
		if(eixos == RotationAxis.MouseX){
			rotacaoX += Input.GetAxis("Mouse X") * sensX;
			rotacaoX = LimitarAngulos(rotacaoX,minimoX,maximoX);
			Quaternion xQuaternion = Quaternion.AngleAxis(rotacaoX, Vector3.up);
			transform.localRotation = xQuaternion * rotacaoOriginal;
		}

		if(eixos == RotationAxis.MouseY){
			rotacaoY += Input.GetAxis("Mouse Y") * sensY;
			rotacaoY = LimitarAngulos(rotacaoY, minimoY, maximoY);
			Quaternion yQuaternion = Quaternion.AngleAxis(-rotacaoY, Vector3.right);
			transform.localRotation = yQuaternion * rotacaoOriginal;
		}

	}
}
