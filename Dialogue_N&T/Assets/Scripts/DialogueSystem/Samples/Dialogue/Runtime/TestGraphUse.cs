using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using VisualGraphRuntime;

public class TestGraphUse : VisualGraphMonoBehaviour<DialogGraph>
{
    public TextMeshProUGUI Text;
    public Button NextButton;
    public Transform OptionButtonParent;
    public GameObject OptionButton;
    private StringBuilder stringBuilder;
    public EasyObjectPool pool;
    public AudioSource AudioSource;
    public GameObject RoleImagesParent;
    public List<RoleImage> RoleImages = new List<RoleImage>();
    private void OnEnable()
    {
        NextButton.onClick.AddListener(() => {
            if (Graph.currentState.Outputs.First().Connections.Count > 0)
            {
                var port = Graph.currentState.Outputs.First();
                OnSelectNode((DialogNode)port.Connections[0].Node);
            }
        });
    }
    private void OnDisable()
    {
        NextButton.onClick.RemoveAllListeners();
    }
    protected override void Start()
    {
        base.Start();
        Refresh();
    }

    public void Refresh()
    {
        if (Graph != null)
        {
            pool.Recycle();
            AudioSource.Stop();
            AudioSource.clip = null;

            // 文字
            #region
            stringBuilder = new StringBuilder();
            for (int i = 0; i < Graph.currentState.storyData.Count; i++)
            {
                if (Graph.currentState.storyData[i].ConditionConfig == null || Graph.currentState.storyData[i].ConditionConfig.can)
                {
                    stringBuilder.AppendLine(Graph.currentState.storyData[i].Text);
                }
            }
            Text.text = stringBuilder.ToString();
            #endregion
            // 立绘
            #region
            if (Graph.currentState.RolePosition != RolePositionEnum.None)
            {
                int currentPosIndex = (int)Graph.currentState.RolePosition -1;
                for (int i = 0; i < RoleImages.Count; i++)
                {
                    if (i == currentPosIndex)
                    {
                        RoleImages[i].Init();
                    }
                    else
                    {
                        RoleImages[i].Dark();
                    }
                }
            }
            else
            {
                RoleImagesParent.SetActive(false);
            }
            #endregion
            // 动效

            // 音效
            #region
            if (Graph.currentState.Audio != null)
            {
                AudioSource.clip = Graph.currentState.Audio;
                AudioSource.Play();
            }
            else
            {
                this.AudioSource.clip = null;
            }
            #endregion
            // 按钮
            #region
            foreach (var port in Graph.currentState.Outputs)
            {
                foreach (var Connection in port.Connections)
                {
                    switch (((BaseNode)Connection.Node).NodeEnum)
                    {
                        case DialogNodeEnum.OptionNode:
                            NextButton.gameObject.SetActive(false);
                            var go = pool.Spawn("OptionButton").gameObject;
                            var button = go.GetComponent<Button>();
                            button.onClick.RemoveAllListeners();
                            button.onClick.AddListener(() => {
                                if (Connection.Node.Outputs.First().Connections.Count > 0)
                                {
                                    OnSelectNode((DialogNode)Connection.Node.Outputs.First().Connections[0].Node);
                                }
                            });
                            button.transform.SetParent(OptionButtonParent);
                            button.GetComponentInChildren<TextMeshProUGUI>().text = ((OptionNode)Connection.Node).Text;
                            go.SetActive(true);
                            break;

                        case DialogNodeEnum.StoryNode:
                            NextButton.gameObject.SetActive(true);
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion
        }
    }

    public void OnSelectNode(DialogNode Node)
    {
        Graph.currentState = Node;
        Refresh();
    }
}
