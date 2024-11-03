using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject character; // Capsule + Sphere (Corps + T�te)
    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;
    public float baseMoveSpeed = 5f; // Vitesse de base
    public float sprintSpeedMultiplier = 1.5f; // Multiplicateur de vitesse pour le sprint
    public float transitionDuration = 0.5f;

    private Vector3 destination;
    private bool isMoving = false;
    private bool isControlMode = false;
    private bool isFreeMode = false; // Indique si le mode libre est actif
    private bool isFirstPerson = false;

    public LayerMask terrainLayer;

    void Start()
    {
        firstPersonCamera.enabled = false;
        thirdPersonCamera.enabled = true;
        character.SetActive(false);
    }

    void Update()
    {
        HandleControlModeToggle();
        HandleCameraSwitch();

        if (isFreeMode)
        {
            HandleFreeModeMovement();
        }
        else
        {
            HandleMovement();
        }
    }

    void HandleControlModeToggle()
    {
        // Basculer le mode de contr�le avec F2 pour le d�placement vers une destination
        if (Input.GetKeyDown(KeyCode.F2))
        {
            isControlMode = !isControlMode;
            isFreeMode = false;
            character.SetActive(isControlMode);
            Debug.Log("Mode destination activ� : " + isControlMode);
        }

        // Activer/d�sactiver le mode libre avec F3
        if (Input.GetKeyDown(KeyCode.F3))
        {
            isFreeMode = !isFreeMode;
            isControlMode = false;
            character.SetActive(isFreeMode);
            Debug.Log("Mode libre activ� : " + isFreeMode);

            if (isFreeMode)
            {
                thirdPersonCamera.enabled = true; // Active la cam�ra 3�me personne par d�faut
            }
            else
            {
                ExitFreeMode();
            }
        }

        // Quitter le mode libre avec �chap
        if (Input.GetKeyDown(KeyCode.Escape) && isFreeMode)
        {
            ExitFreeMode();
            Debug.Log("Mode libre d�sactiv� avec �chap");
        }
    }

    void HandleCameraSwitch()
    {
        // Basculer entre les cam�ras � la premi�re et � la troisi�me personne avec Left Control
        if (Input.GetKeyDown(KeyCode.LeftControl) && isFreeMode)
        {
            isFirstPerson = !isFirstPerson;
            StartCoroutine(SwitchCamera(isFirstPerson));
        }
    }

    // D�placement vers une destination (mode F2)
    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0) && !isFreeMode)
        {
            Ray ray = (thirdPersonCamera.enabled ? thirdPersonCamera : firstPersonCamera).ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    destination = hit.point;
                    isMoving = true;
                    OrientCharacter(destination);
                }
            }
        }

        if (isControlMode || isMoving)
        {
            MoveCharacter();
        }
    }

    // D�placement libre avec ZQSD, fl�ches, et sprint avec Shift gauche
    void HandleFreeModeMovement()
    {
        float speed = baseMoveSpeed;

        // Augmentation de la vitesse avec Shift gauche
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= sprintSpeedMultiplier;
        }

        // Entr�es pour les d�placements libres avec ZQSD et fl�ches
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calcul de la direction de d�placement
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        character.transform.Translate(direction * speed * Time.deltaTime, Space.Self);
    }

    // Tourner le personnage vers la destination et s'orienter selon la pente
    void OrientCharacter(Vector3 target)
    {
        Vector3 direction = (target - character.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        character.transform.rotation = Quaternion.Slerp(character.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void MoveCharacter()
    {
        if (isMoving)
        {
            float step = baseMoveSpeed * Time.deltaTime;
            character.transform.position = Vector3.MoveTowards(character.transform.position, destination, step);

            if (Vector3.Distance(character.transform.position, destination) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    IEnumerator SwitchCamera(bool firstPerson)
    {
        float elapsedTime = 0;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        firstPersonCamera.enabled = firstPerson;
        thirdPersonCamera.enabled = !firstPerson;
    }

    // Sortie du mode libre
    void ExitFreeMode()
    {
        isFreeMode = false;
        character.SetActive(false);
    }
}
