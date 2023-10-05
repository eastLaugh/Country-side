using System;
using System.Collections;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour
{
	public static event Action<Story> OnCreateStory;
	[SerializeField] private TextMeshProUGUI storyText;
	bool LOCKED = false;
	private void Awake()
	{
		Story = new Story(inkJSONAsset.text);
		Story.BindExternalFunction("InfoWindowCreate", (string text) =>
		{
			InfoWindow.Create(text);
			return null;
		});

		Story.BindExternalFunction("Check", (string text) =>
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
	}

	void LOCK(bool state)
	{
		LOCKED = state;
		foreach (Button btn in Btns.GetComponentsInChildren<Button>())
		{
			btn.interactable = !LOCKED;
		}
	}

	void Start()
	{
		// Remove the default message
		RemoveChildren();
		StartStory();
	}

	// Creates a new Story object with the compiled story which we can then play!
	void StartStory()
	{
		OnCreateStory?.Invoke(Story);
		RefreshView();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView()
	{
		// Remove all the UI on screen
		RemoveChildren();

		// Read all the content until we can't continue any more
		while (Story.canContinue)
		{
			// Continue gets the next line of the story
			string text = Story.Continue();
			// This removes any white space from the text.
			Debug.Log(text);
			text = text.Trim();
			// Display the text on screen!
			if (text.Length > 0) 
				CreateContentView(text);
		}

		// Display all the choices, if there are any!
		if (Story.currentChoices.Count > 0)
		{
			for (int i = 0; i < Story.currentChoices.Count; i++)
			{
				Choice choice = Story.currentChoices[i];
				Button button = CreateChoiceView(choice.text.Trim());
				// Tell the button what to do when we press it
				button.onClick.AddListener(delegate
				{
					OnClickChoiceButton(choice);
					EventHandler.CallInitSoundEffect(SoundName.BtnClick);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else
		{
			// Button choice = CreateChoiceView("End of story.\nRestart?");
			// choice.onClick.AddListener(delegate
			// {
			// 	StartStory();
			// });
			//panel.SetActive(false);
		}
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton(Choice choice)
	{
		Story.ChooseChoiceIndex(choice.index);
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

	[SerializeField]
	private TextAsset inkJSONAsset = null;
	public static Story Story { get; private set; }

	[SerializeField]
	private GameObject Btns = null;

	// UI Prefabs
	[SerializeField]
	private GameObject textPrefab = null;
	[SerializeField]
	private GameObject buttonPrefab = null;
}
