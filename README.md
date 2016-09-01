Calligraphy for Xamarin
===========

A Xamarin binding of the Android Calligraphy library from [Christopher Jenkins](https://github.com/chrisjenx/Calligraphy).

##Getting started

### Install

[Download from NuGet](https://www.nuget.org/packages/CallygraphyXamarin/)

```
Install-Package CallygraphyXamarin
```

### Fonts

Add your custom fonts to `assets/fonts/` all font definitions are relative to this path.

_Note: Be sure to set the build action to AndroidAsset; otherwise Android will default to the theme font.

### Configuration

Define your default font using `CalligraphyConfig`, in your `Application` class, unfortunately 
`Activity.OnCreate(Bundle)` is called _after_ `Activity.AttachBaseContext(Context)` so the config 
needs to be defined before that.

```csharp
public override void OnCreate()
        {
            base.OnCreate();
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                    .SetDefaultFontPath("fonts/gtw.ttf")
                    .SetFontAttrId(Resource.Attribute.fontPath)
                // Adding a custom view that support adding a typeFace
                    // .AddCustomViewWithSetTypeface(Java.Lang.Class.FromType(typeof(CustomViewWithTypefaceSupport)))
                // Adding a custom style
                    // .AddCustomStyle(Java.Lang.Class.FromType(typeof(TextField)), Resource.Attribute.textFieldStyle)
                .Build()
            );
        }
}
```
_Note: You don't need to define `CalligraphyConfig` but the library will apply
no default font. I recommend defining at least a default font or attribute._

### Inject into Context

Wrap the Activity Context:

```csharp
        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }
```

_You're ready to go!_


## Usage

### Custom font per TextView

```xml
<TextView
    android:text="@string/hello_world"
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    fontPath="fonts/Roboto-Bold.ttf"/>
```

### Custom font in TextAppearance


```xml
<style name="TextAppearance.FontPath" parent="android:TextAppearance">
    <!-- Custom Attr-->
    <item name="fontPath">fonts/RobotoCondensed-Regular.ttf</item>
</style>
```

```xml
<TextView
    android:text="@string/hello_world"
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:textAppearance="@style/TextAppearance.FontPath"/>

```

### Custom font in Styles


```xml
<style name="TextViewCustomFont">
    <item name="fontPath">fonts/RobotoCondensed-Regular.ttf</item>
</style>
```

### Custom font defined in Theme

```xml
<style name="AppTheme" parent="android:Theme.Holo.Light.DarkActionBar">
    <item name="android:textViewStyle">@style/AppTheme.Widget.TextView</item>
</style>

<style name="AppTheme.Widget"/>

<style name="AppTheme.Widget.TextView" parent="android:Widget.Holo.Light.TextView">
    <item name="fontPath">fonts/Roboto-ThinItalic.ttf</item>
</style>
```

#FAQ

### Font Resolution 

The `CalligraphyFactory` looks for the font in a pretty specific order, for the _most part_ it's
 very similar to how the Android framework resolves attributes.
 
1. `View` xml - attr defined here will always take priority.
2. `Style` xml - attr defined here is checked next.
3. `TextAppearance` xml - attr is checked next, the only caveat to this is **IF** you have a font 
 defined in the `Style` and a `TextAttribute` defined in the `View` the `Style` attribute is picked first!
4. `Theme` - if defined this is used.
5. `Default` - if defined in the `CalligraphyConfig` this is used of none of the above are found 
**OR** if one of the above returns an invalid font. 

#Credits

- [@chrisjenx](https://github.com/chrisjenx)

#Licence

    Copyright 2014 Michele Scandura
    
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at
    
        http://www.apache.org/licenses/LICENSE-2.0
    
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.

