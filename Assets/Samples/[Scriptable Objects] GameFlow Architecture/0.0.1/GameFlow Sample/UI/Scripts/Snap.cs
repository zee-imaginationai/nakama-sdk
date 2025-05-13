using UnityEngine;

public class Snap
{
    public int CurrentRect { get; private set;}
    private RectTransform[] _content;
    private RectTransform _scrollView;
    private int _centerIndex;
    private int _snapIndex;
    private bool _isDraging;
    private bool _isScrollable;
    private Vector2 _distanceVector;
    private Vector2 _startPosition;
    private Vector2 _endPosition;

    public Snap(RectTransform[] content, RectTransform scroll, int centerIndex)
    {
        _scrollView = scroll;
        _centerIndex = centerIndex;
        _content = content;
        _scrollView.anchoredPosition = _content[_centerIndex].anchoredPosition;
        CurrentRect = _centerIndex;
        _snapIndex = CurrentRect;
        _isDraging = false;
        _isScrollable = false;
        
    }

    public void Snaps()
    {
    
        if(!_isDraging && _isScrollable)
        {
            _scrollView.anchoredPosition = Vector2.Lerp(_scrollView.anchoredPosition, -_content[CurrentRect].anchoredPosition, 0.5f);
        }

        if(_scrollView.anchoredPosition.x < (-_content[CurrentRect].anchoredPosition.x + 0.1f) && _scrollView.anchoredPosition.x > (-_content[CurrentRect].anchoredPosition.x - 0.1f))
        {
            _isScrollable = false;
        }
    }

    public void SetSnapIndex(int i)
    {
        _snapIndex = i;
        CurrentRect = i;
    }


    public void OnDragStart()
    {
        _startPosition = Input.touches[0].position;
        _isDraging = true;
    }

    public void OnDragEnd()
    {
        _endPosition = Input.touches[0].position;
        _distanceVector = _startPosition - _endPosition;
        Debug.Log(_distanceVector);

        if (_distanceVector.x > 200f && CurrentRect < (_content.Length-1))
        {

            CurrentRect++;
        }

        if (_distanceVector.x < -200f && CurrentRect > 0)
        {

            CurrentRect--;
        }
        _isDraging = false;
        _isScrollable = true;
    }



}
