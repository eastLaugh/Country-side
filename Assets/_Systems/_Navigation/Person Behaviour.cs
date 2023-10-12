using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PersonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Person person;
    public Outline outline;

    public Color EnterColor;
    public Color SelectedColor;
    public static PersonBehaviour SelectedPerson { get; private set; }


    public NavMeshAgent agent;
    private void Awake()
    {

    }
    private void OnEnable()
    {
    }


    private void OnDisable()
    {
    }


    private void Update()
    {
        person.worldPosition = transform.position;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (agent.remainingDistance < 0.2f)
            {
                if (person.PathPoints.Count > 0)
                {
                    int index = person.PathPoints.IndexOf(person.destination);
                    if (index == -1)
                    {
                        person.destination = person.PathPoints[0];
                        agent.SetDestination(person.PathPoints[0].worldPosition);
                    }
                    else
                    {
                        person.destination = person.PathPoints[(index + 1) % person.PathPoints.Count];
                        agent.SetDestination(person.destination.worldPosition);
                    }
                }
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (SelectedPerson != this)
        {
            if (SelectedPerson != null)
            {
                SelectedPerson.DeSelect();
            }
            SelectedPerson = this;
            outline.OutlineColor = SelectedColor;
            outline.enabled = true;

            OnSelect();
        }
        else
        {

            DeSelect();

            OnPointerEnter(null);

        }


    }

    bool isRecording = false;
    private void OnGUI()
    {
        if (true)
        {

            if (SelectedPerson == this)
            {
                if (GameManager.DebugMode)
                {

                    Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                    var rect = new Rect(screenPos.x, Screen.height - screenPos.y, 200, 200);
                    // GUI.Box(rect, "Person");
                    GUILayout.BeginArea(rect, GUI.skin.box);

                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(person.name);
                        if (GUILayout.Button("X"))
                        {
                            DeSelect();
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (!isRecording && GUILayout.Button("录入路径点"))
                    {
                        SlotRender.OnAnySlotClicked += OnAnySlotClicked;
                        isRecording = true;
                    }
                    if (isRecording && GUILayout.Button("结束录入"))
                    {
                        SlotRender.OnAnySlotClicked -= OnAnySlotClicked;
                        isRecording = false;
                    }
                    if (!isRecording && GUILayout.Button("清除所有已经录入的路径点"))
                    {
                        person.PathPoints.Clear();
                    }

                    GUILayout.Label("");
                    GUILayout.EndArea();
                }
                else
                {

                }

            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SelectedPerson != this)
        {
            outline.OutlineColor = EnterColor;
            outline.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectedPerson != this)
        {
            outline.enabled = false;
        }
    }

    void OnSelect()
    {
        if (GameManager.DebugMode)
        {

        }
        else
        {

        }
        ChatWindow.OpenOrCreate(person);
    }

    void DeSelect()
    {
        if (SelectedPerson == this)
            SelectedPerson = null;
        outline.enabled = false;

        if (GameManager.DebugMode)
        {

        }
        else
        {

        }
        ChatWindow.Close(person);
    }

    private void OnAnySlotClicked(SlotRender render)
    {
        person.AddPathPoint(render.slot);

        // agent.SetDestination(render.slot.worldPosition);
    }
}