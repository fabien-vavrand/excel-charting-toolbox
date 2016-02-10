#Excel Charting Toolbox
Excel Charting Toolbox is an Excel Add-In providing advanced and intuitive charting capabilities inside Excel.
##Treemap
Create a treemap based on selected data, and choose which columns generate levels, sizes and colors.

* **Multi levels**: treemap can have multiple levels
* **Design**: each level has multiple options to precisely tune your treemap look
* **Colors**: treemap colors can be defined with different options: 2/3 colors gradient or palette
* Easily insert Treemap generation into your automatized reportings

```
TreemapParameters parameters = new TreemapParameters()
      .AddIndex(new TreemapIndex()
      {
          LineVisible = false,
          FontBold = false,
          FontColor = Color.Black
      })
      .WithColor(
          new ColorGradient()
              .AddStop(0, Color.White)
              .AddStop(1, Color.Red)
      );
  TreemapChart treemap = new TreemapChart<UsRegion>(
          data,
          r => new List<string>(r.Region),
          r => r.Area,
          r => r.Population,
          parameters)
      .Build(0, 0, 600, 250)
      .Print(sheet);
```

![](docs/Images/treemap1.png)
