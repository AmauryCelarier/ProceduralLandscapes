using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject character; // Capsule + Sphere (Corps + T�te)
    public Camera thirdPersonCamera;
    public Camera firstPersonCamera;
    public float moveSpeed = 5f;
    public float transitionDuration = 0.5f;

    private Vector3 destination;
    private bool isMoving = false;
    private bool isControlMode = false;
    private bool isFirstPerson = false;

    public LayerMask terrainLayer;  // Ajoute cette ligne pour assigner le Layer dans Unity


    void Start()
    {
        // D�finir l'�tat initial : d�sactiver le personnage, activer la cam�ra � la troisi�me personne
        firstPersonCamera.enabled = false;
        thirdPersonCamera.enabled = true;
        character.SetActive(false); // Masquer le personnage initialement
    }

    void Update()
    {
        HandleControlModeToggle();
        HandleCameraSwitch();
        HandleMovement();
    }

    void HandleControlModeToggle()
    {
        // Basculer le mode de contr�le avec F2
        if (Input.GetKeyDown(KeyCode.F2))
        {
            isControlMode = !isControlMode;
            character.SetActive(isControlMode);
        }

        // Quitter le mode de contr�le avec �chap
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isControlMode = false;
            character.SetActive(false);
        }
    }

    void HandleCameraSwitch()
    {
        // Basculer entre les cam�ras � la premi�re et � la troisi�me personne avec Left Control
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isFirstPerson = !isFirstPerson;
            StartCoroutine(SwitchCamera(isFirstPerson));
        }
    }

    void HandleMovement()
    {
        // D�placer le personnage avec un clic gauche de la souris lorsque le mode de contr�le est actif
        if (Input.GetMouseButtonDown(0))
        {
            //S'adapte en fonction de si la camera est TPS ou FPS
            Ray ray;
            if (thirdPersonCamera.enabled)
            {
                ray = thirdPersonCamera.ScreenPointToRay(Input.mousePosition);
            }
            else { ray = firstPersonCamera.ScreenPointToRay(Input.mousePosition); }
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
            {
                if (hit.collider.CompareTag("Terrain"))
                {
                    destination = hit.point;
                    isMoving = true;
                    OrientCharacter(destination);
                }
            }
            else
            {
                Debug.Log("Raycast didn't hit anything"); // Affiche si rien n'est touch�
            }
        }

        if (isControlMode || isMoving)
        {
            MoveCharacter();
        }
    }

    // Tourner le personnage vers la destination
    void OrientCharacter(Vector3 target)
    {
        Vector3 direction = (target - character.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        character.transform.rotation = Quaternion.Slerp(character.transform.rotation, lookRotation, Time.deltaTime * 5f);
        Debug.Log("Orienting character towards target at: " + target);
    }

    // D�placer le personnage vers la destination
    void MoveCharacter()
    {
        if (isMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            character.transform.position = Vector3.MoveTowards(character.transform.position, destination, step);
            Debug.Log("Moving character to: " + destination);  // V�rifie que le mouvement est bien effectu�

            if (Vector3.Distance(character.transform.position, destination) < 0.1f)
            {
                isMoving = false;
                Debug.Log("Character arrived at destination");  // V�rifie que l'arr�t du mouvement est correct
            }
        }
    }


    // Transition fluide de la cam�ra entre la premi�re et la troisi�me personne
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
}