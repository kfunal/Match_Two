#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class EditorHelper : IDisposable
{
    // This class created to increase the code readability and  to avoid code duplication

    private static EditorHelper instance;
    public static EditorHelper Instance
    {
        get
        {
            if (instance == null)
                instance = new EditorHelper();

            return instance;
        }
    }

    /// <summary>
    /// Creates horizontal layout
    /// </summary>
    /// <param name="_content"></param>
    public void HorizontalLayout(Action _content)
    {
        EditorGUILayout.BeginHorizontal();
        _content?.Invoke();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Create horizontal layout by given size
    /// </summary>
    /// <param name="_content"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    public void HorizontalLayout(float _width, float _height, Action _content)
    {
        EditorGUILayout.BeginHorizontal(new GUIStyle()
        {
            fixedHeight = _height,
            fixedWidth = _width
        });
        _content?.Invoke();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Creates horizontal layout and with flexible space to center the element.
    /// </summary>
    /// <param name="_element"></param>
    public void CenteredHorizontalLayout(Action _layoutContent)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        _layoutContent?.Invoke();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Creates horizontal layout and with flexible space to center the element.
    /// </summary>
    /// <param name="_element"></param>
    public void CenteredHorizontalLayout(float _width, float _height, Action _layoutContent)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        HorizontalLayout(_width, _height, () => _layoutContent?.Invoke());
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Creates label without space afterwords with given width and height or default size
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_width"></param>
    /// <param name="_height"></param>
    public void LabelWithoutSpace(string _text, float _width, float _height)
    {
        GUILayout.Label(_text, new GUIStyle(GUI.skin.label)
        {
            stretchWidth = false,
            stretchHeight = false,
            fixedHeight = _height,
            fixedWidth = _width,
        });
    }

    /// <summary>
    /// Creates button grid. Since editor renderer top to bottom, in first for loop we go through end to start to make row and column truly.
    /// </summary>
    /// <param name="_row"></param>
    /// <param name="_column"></param>
    /// <param name="_content"></param>
    public void ButtonGrid(int _row, int _column, Action<int, int> _content)
    {
        for (int row = _row; row >= 0; row--)
        {
            CenteredHorizontalLayout(() =>
            {
                for (int column = 0; column < _column; column++)
                {
                    _content?.Invoke(row, column);
                }
            });
        }
    }

    /// <summary>
    /// Converts given int to enum with given type33333
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_value"></param>
    /// <returns></returns>
    public T IntToEnum<T>(int _value)
    {
        return (T)Enum.ToObject(typeof(T), _value);
    }


    /// <summary>
    /// Converts given sprite to texture
    /// </summary>
    /// <param name="_sprite"></param>
    /// <returns></returns>
    public Texture SpriteToTexture(Sprite _sprite)
    {
        var croppedTexture = new Texture2D((int)_sprite.rect.width, (int)_sprite.rect.height);
        var pixels = _sprite.texture.GetPixels((int)_sprite.textureRect.x,
                                                (int)_sprite.textureRect.y,
                                                (int)_sprite.textureRect.width,
                                                (int)_sprite.textureRect.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        return croppedTexture;
    }

    public void Button(Texture _content, float _width, float _height, int _buttonIndex, Action<int> _onClick)
    {
        GUIContent content = new GUIContent(_content);
        GUIStyle style = new GUIStyle(GUI.skin.button)
        {
            fixedHeight = _height,
            fixedWidth = _width
        };

        if (GUILayout.Button(content, style))
            _onClick?.Invoke(_buttonIndex);
    }

    public void CenteredBox(Texture _content, float _width, float _height)
    {
        CenteredHorizontalLayout(() =>
        {
            GUIContent content = new GUIContent(_content);
            GUIStyle style = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = _height,
                fixedWidth = _width
            };

            GUILayout.Box(content, style);
        });
    }

    public void Dispose()
    {
        instance = null;
    }
}
#endif