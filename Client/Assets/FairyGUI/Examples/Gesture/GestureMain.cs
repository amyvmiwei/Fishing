﻿using UnityEngine;
using FairyGUI;
using DG.Tweening;

public class GestureMain : MonoBehaviour
{
    GComponent _mainView;
    Transform _ball;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Stage.inst.onKeyDown.Add(OnKeyDown);

        UIPackage.AddPackage("UI/Gesture");
    }

    void Start()
    {
        _mainView = this.GetComponent<UIPanel>().ui;
        GObject holder = _mainView.GetChild("holder");

        _ball = GameObject.Find("Globe").transform;

        FairyGUI.SwipeGesture gesture1 = new FairyGUI.SwipeGesture(holder);
        gesture1.onMove.Add(OnSwipeMove);
        gesture1.onEnd.Add(OnSwipeEnd);

        FairyGUI.LongPressGesture gesture2 = new FairyGUI.LongPressGesture(holder);
        gesture2.once = false;
        gesture2.onAction.Add(OnHold);

        FairyGUI.PinchGesture gesture3 = new FairyGUI.PinchGesture(holder);
        gesture3.onAction.Add(OnPinch);

        FairyGUI.RotationGesture gesture4 = new FairyGUI.RotationGesture(holder);
        gesture4.onAction.Add(OnRotate);
    }

    void OnSwipeMove(EventContext context)
    {
        FairyGUI.SwipeGesture gesture = (FairyGUI.SwipeGesture)context.sender;
        Vector3 v = new Vector3();
        if (Mathf.Abs(gesture.delta.x) > Mathf.Abs(gesture.delta.y))
        {
            v.y = -Mathf.Round(gesture.delta.x);
            if (Mathf.Abs(v.y) < 2) //消除手抖的影响
                return;
        }
        else
        {
            v.x = -Mathf.Round(gesture.delta.y);
            if (Mathf.Abs(v.x) < 2)
                return;
        }
        _ball.Rotate(v, Space.World);
    }

    void OnSwipeEnd(EventContext context)
    {
        FairyGUI.SwipeGesture gesture = (FairyGUI.SwipeGesture)context.sender;
        Vector3 v = new Vector3();
        if (Mathf.Abs(gesture.velocity.x) > Mathf.Abs(gesture.velocity.y))
        {
            v.y = -Mathf.Round(Mathf.Sign(gesture.velocity.x) * Mathf.Sqrt(Mathf.Abs(gesture.velocity.x)));
            if (Mathf.Abs(v.y) < 2)
                return;
        }
        else
        {
            v.x = -Mathf.Round(Mathf.Sign(gesture.velocity.y) * Mathf.Sqrt(Mathf.Abs(gesture.velocity.y)));
            if (Mathf.Abs(v.x) < 2)
                return;
        }
        _ball.DORotate(v, 0.3f, RotateMode.WorldAxisAdd);
    }

    void OnHold(EventContext context)
    {
        _ball.DOShakePosition(0.3f, new Vector3(0.1f, 0.1f, 0));
    }

    void OnPinch(EventContext context)
    {
        DOTween.Kill(_ball);

        FairyGUI.PinchGesture gesture = (FairyGUI.PinchGesture)context.sender;
        float newValue = Mathf.Clamp(_ball.localScale.x + gesture.delta, 0.3f, 2);
        _ball.localScale = new Vector3(newValue, newValue, newValue);
    }

    void OnRotate(EventContext context)
    {
        DOTween.Kill(_ball);

        RotationGesture gesture = (RotationGesture)context.sender;
        _ball.Rotate(Vector3.forward, -gesture.delta, Space.World);
    }

    void OnKeyDown(EventContext context)
    {
        if (context.inputEvent.keyCode == KeyCode.Escape)
        {
            Application.Quit();
        }
    }
}