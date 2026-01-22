using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

    #region Variables declaration
    public float MaxSpeed = 10f;
    private float currentSpeed = 0f; // Vitesse actuelle
    private float targetSpeed = 0f; // Vitesse cible en fonction de la vitesse max

    public AnimationCurve accelValue; // Courbe d'accélération
    private float accelTime = 0f;
    public int accelFrameDuration = 10;

    public AnimationCurve decelValue; // Courbe de deccélération
    private float decelTime = 0f;
    public int decelFrameDuration = 10;



    private InputSystem_Actions controls;
    private Vector2 moveInput;

    private Rigidbody2D rb;

    public float boundary;
    [Range(0f, 1f)]
    public float boundaryPercentage = 0.8f;

    private enum PlayerState
    {
        Idle,
        Accelerating,
        Decelerating
    }

    private PlayerState currentState = PlayerState.Idle; 
    #endregion


    #region Initialize Region
    private void Awake()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody2D>();
        // Initialisation des inputs
        controls = new InputSystem_Actions();
        controls.Player.Move.performed += ctx => HandheldMovePressed(ctx);
        controls.Player.Move.canceled += ctx => HandheldMoveRelease(ctx);

        CalculateBoundary();
    }



    private void OnEnable()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    } 
    #endregion

    // Update is called once per frame
    void FixedUpdate()
    {

        if (GameManager.Instance.IsPaused) return;

        if(Mathf.Approximately(targetSpeed - currentSpeed, 0) && currentState != PlayerState.Idle)
        {
            decelTime = 0f;
            currentState = PlayerState.Idle;
        }
        else if (moveInput.x != 0 && currentState != PlayerState.Accelerating)
        {
            accelTime = 0f;
            currentState = PlayerState.Accelerating;
        }
        else if (moveInput.x == 0 && !Mathf.Approximately(currentSpeed,0) && currentState != PlayerState.Decelerating)
        {
            decelTime = 0f;
            currentState = PlayerState.Decelerating;
            Debug.Log("😒 Transition vers Decelerating");
        }
        // Appliquer le comportement en fonction de l'état
        switch (currentState)
        {
            case PlayerState.Accelerating:
                HandledAcceleration(); 
                break;
            
            case PlayerState.Decelerating:
                HandledDeceleration();
                break;
            
            case PlayerState.Idle:
                break;
        }
        // Calcul de la position
        float newXPosition = rb.position.x + (currentSpeed * Time.fixedDeltaTime);

        
        if(newXPosition <= -boundary || newXPosition >= boundary)
        {
            currentSpeed = 0f;

            newXPosition = Mathf.Clamp(newXPosition, -boundary, boundary);
        }

        rb.MovePosition(new Vector2(newXPosition, rb.position.y));

        Debug.Log($"State : {currentState}, Current Speed : {currentSpeed}, Target Speed : {targetSpeed}, Move Input :! {moveInput}, Decel Time : {decelTime}, Accel Time : {accelTime}");
       
    }

    private void HandheldMovePressed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>(); // Récupère l'input Move de l'utilisateur.
        targetSpeed = Mathf.Clamp(moveInput.x * MaxSpeed, -MaxSpeed, MaxSpeed);
        accelTime = 0f;

    }

    private void HandheldMoveRelease(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero; // (0,0)
        targetSpeed = 0f;
        decelTime = 0f;
    }

    private void HandledAcceleration()
    {
        float curveTimeScale = accelValue.keys[accelValue.length - 1].time;
        accelTime += Time.fixedDeltaTime / ((accelFrameDuration * Time.fixedDeltaTime) / curveTimeScale);

        float curveValue = accelValue.Evaluate(accelTime);

        float minCurveValue = 0.2f;
        curveValue = Mathf.Max(curveValue, minCurveValue);

        currentSpeed += Mathf.Sign(targetSpeed) * curveValue * MaxSpeed * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -MaxSpeed, MaxSpeed);

    }
    private void HandledDeceleration()
    {
        // TODO : Implémenter la décélération
        float curveTimeScale = decelValue.keys[decelValue.length - 1].time; // Durée totale de la décélération

        decelTime += Time.fixedDeltaTime / ((decelFrameDuration * Time.fixedDeltaTime)/ curveTimeScale);

        float curveValue = decelValue.Evaluate(decelTime);

        float minCurveValue = 0.2f;
        curveValue = Mathf.Max(curveValue, minCurveValue);

        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.Clamp(currentSpeed - curveValue * MaxSpeed * Time.fixedDeltaTime, 0, MaxSpeed);

        }
        else if (currentSpeed < 0)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + curveValue * MaxSpeed * Time.fixedDeltaTime, -MaxSpeed, 0);

        }

        if (Mathf.Abs(currentSpeed) <= 0.5f && Mathf.Approximately(targetSpeed, 0))
        {
            currentSpeed = 0f;
            decelTime = 0f;
        }
    }

    #region Calcul les boundaries & Draw les gizmos
    private void CalculateBoundary()
    {
        // Obtenir la largeur visible de l'écran en coordonnées du monde
        Vector3 screenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));

        // Calculer boundary en fonction du pourcentage de l'écran
        boundary = screenBounds.x * boundaryPercentage;

        Debug.Log($"😊 Boundary calculé : {boundary}. Parce que j'ai calculé avec le pourcentage {boundaryPercentage}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Gizmo.DrawLine(startposition, endposition)
        Gizmos.DrawLine(new Vector3(-boundary, -10, 0), new Vector3(-boundary, 10, 0));
        Gizmos.DrawLine(new Vector3(boundary, -10, 0), new Vector3(boundary, 10, 0));
    } 
    #endregion
}
