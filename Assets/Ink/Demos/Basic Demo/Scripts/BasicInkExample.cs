using System;
using System.Collections;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour, IPointerClickHandler
{
    Map map;
    public bool isMaploaded = false;
    [SerializeField] private TextMeshProUGUI storyText;
	[SerializeField] private TextMeshProUGUI nameText;
    [SerializeField]
    private TextAsset Guide1JSON = null;
    [SerializeField]
    private TextAsset Guide2JSON = null;
	[SerializeField] private TourGuideUI tourGuideUI;
    [SerializeField]
    private GameObject Btns = null;
    // UI Prefabs
    [SerializeField]
    private GameObject textPrefab = null;
    [SerializeField]
    private GameObject buttonPrefab = null;
	[SerializeField]
	private GameObject AssignmentWindow;
    public static Story CurrentStory { get; private set; }
	private Story Guide1;
	private Story Guide2;
    public static event Action<Story> OnCreateStory;
    bool LOCKED = false;
	bool isChoices;
	private void Awake()
	{
		Guide1 = new Story(Guide1JSON.text);
        Guide2 = new Story(Guide2JSON.text);
		BindFunc(Guide1);
		BindFunc(Guide2);
    }
	void BindFunc(Story story)
	{
        story.BindExternalFunction("InfoWindowCreate", (string text) =>
        {
            InfoWindow.Create(text);
            return null;
        });

        story.BindExternalFunction("Check", (string text) =>
        {
            bool finished = false;
            AssignmentSystem.assignmentList.ForEach((BasicAssignment assignment) =>
            {
                if (assignment.name == text)
                {
                    if (assignment.finished) finished = true;
                    assignment.onAssignmentFinished += () => LOCK(false);
                }
            });
            LOCK(!finished);
        });
        story.BindExternalFunction("SetName", (string text) =>
        {
            nameText.text = text;
        });
		story.BindExternalFunction("GiveAssignmentHint", () =>
		{
			if(AssignmentSystem.displayList.Count > 0)
			{
                int index = UnityEngine.Random.Range(0, AssignmentSystem.displayList.Count);
                storyText.text = AssignmentSystem.displayList[index].hint;
            }
			else
			{
				storyText.text = "感谢你，年轻人。";

            }
			
		});
		story.BindExternalFunction("ShowAssignment", () =>
		{
			AssignmentWindow.SetActive(true);
		});
    }
	void LOCK(bool state)
	{
		LOCKED = state;
		foreach (Button btn in Btns.GetComponentsInChildren<Button>())
		{
			btn.interactable = !LOCKED;
		}
	}
    private void OnEnable()
    {
        GameManager.OnMapLoaded += OnMapLoaded;
        GameManager.OnMapUnloaded += OnMapUnloaded;

        // Debug.Log("OnEable");
    }
    private void OnDisable()
    {
        GameManager.OnMapUnloaded -= OnMapUnloaded;
        //Debug.Log("Ondisable");
    }
    private void OnMapUnloaded()
    {
        isMaploaded = false;
    }

    private void OnMapLoaded(Map map)
    {
        this.map = map;
        isMaploaded = true;
		if(!map.isTurtorialDone) 
		{
            CurrentStory = Guide1;
            RemoveChildren();
            StartStory();
        }
		else
		{
			CurrentStory = Guide2;
            RemoveChildren();
            StartStory();
        }
    }
    void Start()
	{
		// Remove the default message
		
	}

	// Creates a new Story object with the compiled story which we can then play!
	void StartStory()
	{
		OnCreateStory?.Invoke(CurrentStory);
		RefreshView();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView()
	{
		// Remove all the UI on screen
		RemoveChildren();
		bool restart = false;
		// Read all the content until we can't continue any more
		if(CurrentStory.canContinue)
		{
			// Continue gets the next line of the story
			string text = CurrentStory.Continue();
			// This removes any white space from the text.
			//Debug.Log(text);
			text = text.Trim();
			// Display the text on screen!
			if (text.Length > 0) 
				CreateContentView(text);
		}
        else
        {
            // Button choice = CreateChoiceView("End of story.\nRestart?");
            // choice.onClick.AddListener(delegate
            // {
            // 	StartStory();
            // });
            //panel.SetActive(false);
            if (!map.isTurtorialDone)
            {
                CurrentStory = Guide2;
                RemoveChildren();
                StartStory();
                tourGuideUI.CloseWindow();
                map.isTurtorialDone = true;
                restart = true;
                map.Phase = 2;
                EventHandler.CallPhaseUpdate(2);
            }
			else
			{
                CurrentStory.ResetState();
                RemoveChildren();
                StartStory();
				restart = true;
                //tourGuideUI.CloseWindow();
            }
        }
        // Display all the choices, if there are any!
        if (CurrentStory.currentChoices.Count > 0 && !restart)
		{
			isChoices = true;
			for (int i = 0; i < CurrentStory.currentChoices.Count; i++)
			{
				Choice choice = CurrentStory.currentChoices[i];
				Button button = CreateChoiceView(choice.text.Trim());
				// Tell the button what to do when we press it
				button.onClick.AddListener(delegate
				{
					OnClickChoiceButton(choice);
					EventHandler.CallInitSoundEffect(SoundName.BtnClick1);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton(Choice choice)
	{
		CurrentStory.ChooseChoiceIndex(choice.index);
		isChoices = false;
		RefreshView();
	}

	// Creates a textbox showing the the line of text
	void CreateContentView(string text)
	{
		storyText.text = text;
	}

	// Creates a button showing the choice text
	Button CreateChoiceView(string text)
	{
		// Creates the button from a prefab
		Button choice = Instantiate(buttonPrefab).GetComponent<Button>();
		choice.transform.SetParent(Btns.transform, false);
		choice.interactable = !LOCKED;

		// Gets the text from the button prefab
		TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
		choiceText.text = text;

		// Make the button expand to fit the text
		HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
		layoutGroup.childForceExpandHeight = false;

		choice.gameObject.SetActive(true);
		return choice;
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren()
	{
		int childCount = Btns.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i)
		{
			Destroy(Btns.transform.GetChild(i).gameObject);
		}
	}

    public void OnPointerClick(PointerEventData eventData)
    {
       if(!isChoices)
	   {
            RefreshView();
			EventHandler.CallInitSoundEffect(SoundName.BtnClick2);
        }
			
    }
}
