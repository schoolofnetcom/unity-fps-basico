using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour {

	// Variaveis de Transform
	private Transform fpsView;
	private Transform fpsCamera;
	private Vector3 fpsRotation = Vector3.zero;

	// Velocidades
	public float velocidadeAndar = 7.0f;
	public float velocidadeCorrendo = 10.0f;
	public float pulo = 8.0f;
	public float gravidade = 20f;

	private float velocidade;

	// Input && Teclas
	private float inputX, inputY;
	private float inputX_Set, inputY_Set;
	private float inputFator;
	private bool limitarVelocidadeDiagonal = true;
	private float antiBump = 0.75f;

	// Lógicas

	private bool noChao, seMovendo;

	// Outras

	private CharacterController charController;
	private Vector3 movimentoDirecao = Vector3.zero;

	// Agachar, Pular & Correr

	public LayerMask camadaDoChao;
	private float rayDistance;
	private float alturaPadrao;
	private Vector3 camPosPadrao;
	private float camAltura;
	private bool estaAgachado;
	private float velocidadeAgachado = 3.5f;

	// Use this for initialization
	void Start () {
		fpsView = transform.Find("Visao").transform;
		charController = GetComponent<CharacterController>();
		velocidade = velocidadeAndar;
		seMovendo = false;

		// Dividindo
		rayDistance = charController.height * 0.5f + charController.radius;
		alturaPadrao = charController.height;
		camPosPadrao = fpsView.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		Movimento();
	}
	
   void Movimento(){

	   // Detecção de movimento no eixo Y(Vertical)
	   if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)){
		   if(Input.GetKey(KeyCode.W)){
			   inputY_Set = 1f;
		   }else{
			   inputY_Set = -1f;
		   }
	   }else{
		   inputY_Set = 0f;
	   }

		// Detecção de movimento no eixo X(Horizontal)
	   if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)){
		   if(Input.GetKey(KeyCode.D)){
			   inputX_Set = 1f;
		   }else{
			   inputX_Set = -1f;
		   }
	   }else{
		   inputX_Set = 0f;
	   }

	   inputY = Mathf.Lerp(inputY, inputY_Set, Time.deltaTime * 20f);
	   inputX = Mathf.Lerp(inputX, inputX_Set, Time.deltaTime * 20f);
	   inputFator = Mathf.Lerp(inputFator,(inputY_Set != 0 && inputX_Set != 0 && limitarVelocidadeDiagonal) ? 0.75f : 1.0f,Time.deltaTime * 20f);

	   fpsRotation = Vector3.Lerp(fpsRotation, Vector3.zero, Time.deltaTime*5f);
	   fpsView.localEulerAngles = fpsRotation;

	   if(noChao){
		   AgachaECorre();
		  movimentoDirecao = new Vector3(inputX * inputFator,-antiBump, inputY * inputFator);
		  movimentoDirecao = transform.TransformDirection(movimentoDirecao) * velocidade;
		  Pulo();
	   }

		movimentoDirecao.y -= gravidade * Time.deltaTime;

		noChao = (charController.Move(movimentoDirecao * Time.deltaTime) & CollisionFlags.Below) != 0;
		seMovendo = charController.velocity.magnitude > 0.15f;
   }



	void AgachaECorre(){
		if(Input.GetKeyDown(KeyCode.C)){
			if(!estaAgachado){
				estaAgachado = true;
			}else{
				if(elePodeSeLevantar()){
					estaAgachado = false;
				}
			}
		}

		StopCoroutine(moveCameraAgachado());
		StartCoroutine(moveCameraAgachado());

		if(estaAgachado){
			velocidade = velocidadeAgachado;
		}else{
			if(Input.GetKey(KeyCode.LeftShift)){
				velocidade = velocidadeCorrendo;
			}else{
				velocidade = velocidadeAndar;
			}
		}

	}

	bool elePodeSeLevantar(){
		Ray rayTopo = new Ray(transform.position, transform.up);
		RaycastHit rayTopoHit;

		if(Physics.SphereCast(rayTopo, charController.radius + 0.05f, out rayTopoHit, rayDistance, camadaDoChao)){
			if(Vector3.Distance(transform.position, rayTopoHit.point) < 2.3f){
				return false;
			}
		}

		return true;
	}

	IEnumerator moveCameraAgachado(){
		charController.height =  estaAgachado ? alturaPadrao / 1.5f : alturaPadrao;
		charController.center = new Vector3(0f, charController.height/2f, 0f);
		camAltura = estaAgachado ? camPosPadrao.y/1.5f : camPosPadrao.y;

		while(Mathf.Abs(camAltura - fpsView.localPosition.y) > 0.01f){
			fpsView.localPosition = Vector3.Lerp(fpsView.localPosition,
			new Vector3(camPosPadrao.x,camAltura,camPosPadrao.z),Time.deltaTime * 11f);
		}

		yield return null;

	}

	void Pulo(){
		if(Input.GetKey(KeyCode.Space)){
			if(estaAgachado){
				if(elePodeSeLevantar()){
					estaAgachado = false;
				StopCoroutine(moveCameraAgachado());
		        StartCoroutine(moveCameraAgachado());
				}
			}else{
				movimentoDirecao.y = pulo;
			}
		}
	}

}
