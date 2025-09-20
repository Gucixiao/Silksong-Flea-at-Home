202509192001

# Flea At Home v0.1.5

**插件简介**  
Flea At Home 是一个用于空洞骑士-丝之歌的小模组，让你拥有一只宠物跳蚤。

---

## 安装方法

1. 将 `FleaAtHome` 文件夹放入 BepInEx 插件目录：BepInEx/plugins
2. 启动游戏，插件会在控制台输出加载信息。

---

## 使用说明

1. **手动生成跳蚤**  
- 按 F9（或 Config 配置的键位/复合键）生成  
- 按 F10 删除  
- 按方向键 / PageUp / PageDown 调整位置  
- 按 End 键恢复到初始位置  

2. **自动生成跳蚤**  
- 进入钟居的时候自动在床上生成一只小跳蚤。按F10删除。
- 每次进入该场景都会重新生成  

3. **调试和日志**  
- 每次移动跳蚤时，当前坐标会输出到 BepInEx 控制台  
- 可以在 Config 中修改生成位置以测试不同场景  

---

## 功能列表

1. **手动生成/删除**

    - **生成跳蚤**：按 `F9`（默认，可通过 Config 修改键位和复合键）  
    - **删除跳蚤**：按 `F10`（默认，可通过 Config 修改键位和复合键）  

2. **自动生成**

    - 在指定场景（默认 `Belltown_Room_Spare`）进入时自动生成跳蚤  
    - 自动生成位置可通过 Config 修改

3. **跳蚤位置调试**

    - 使用方向键和 PageUp/PageDown 调整跳蚤 X/Y/Z 坐标  
    - End 键恢复初始位置  
    - 移动时实时输出当前坐标到日志，方便精确定位  

4. **Config 支持**

    - 可修改自动生成场景名 (`SceneName`)  

    - 可修改自动生成坐标 (`Position`)  

    - 可修改键位绑定，包括复合键 (`KeyboardShortcut`)  

    - 配置示例（`BepInEx/config/com.gcx.fleaathome.cfg`）：

        ```ini
        [AutoSpawn]
        SceneName = Belltown_Room_Spare
        Position = 27.45, 7.68, 0.01
        
        [Keys]
        SpawnFlea = F9
        DestroyFlea = F10
        ResetPosition = End
        MoveUp = UpArrow
        MoveDown = DownArrow
        MoveLeft = LeftArrow
        MoveRight = RightArrow
        MoveZUp = PageUp
        MoveZDown = PageDown
        ```

---

## 更新记录

### v0.1.0
- 插件首次发布，支持手动生成跳蚤在玩家附近。  
- 基本移动调试（方向键/End键复位）。  
- 支持播放指定视频文件作为跳蚤动画。  

### v0.1.2
- 修复生成跳蚤时 RawImage 不显示的问题。  
- 调整跳蚤 Canvas 和 RawImage 尺寸，降低颜色饱和度。  

### v0.1.4
- 添加 F10 删除跳蚤功能。  
- 支持 PageUp/PageDown 调整 Z 轴位置。  
- 方向键移动跳蚤时输出当前坐标到日志。  

### v0.1.5
- 自动生成跳蚤功能：进入指定场景（默认 Belltown_Room_Spare）延迟 0.1 秒生成跳蚤。  
- 支持通过 Config 修改自动生成场景名和坐标。  
- 支持键位绑定自定义，包含复合键（KeyboardShortcut）。  
- 保留手动生成/删除功能。  
- 日志输出跳蚤当前坐标，用于精确调试。
- 目前仅限1只跳蚤。

---

## 注意事项

- 跳蚤仅为视觉对象，不会影响游戏计数器或逻辑  
- 确保视频文件存在，否则跳蚤不会显示  
- 配置修改后需重启游戏以加载新键位或场景设置

# Flea At Home 插件配置指南

本指南说明如何修改 Flea At Home 插件的键位绑定和自动生成设置。配置文件路径通常位于：`BepInEx/config/com.gcx.fleaathome`

打开该文件后，你会看到类似如下内容：

```ini
[AutoSpawn]
SceneName = Belltown_Room_Spare
Position = 27.45, 7.68, 0.01

[Keys]
SpawnFlea = F9
DestroyFlea = F10
ResetPosition = End

MoveUp = LeftControl + UpArrow
MoveDown = LeftControl + DownArrow
MoveLeft = LeftControl + LeftArrow
MoveRight = LeftControl + RightArrow
MoveZUp = LeftControl + PageUp
MoveZDown = LeftControl + PageDown
```

| 配置项      | 默认值                   | 功能         |
| ----------- | ------------------------ | ------------ |
| `MoveUp`    | LeftControl + UpArrow    | 向上移动跳蚤 |
| `MoveDown`  | LeftControl + DownArrow  | 向下移动跳蚤 |
| `MoveLeft`  | LeftControl + LeftArrow  | 向左移动跳蚤 |
| `MoveRight` | LeftControl + RightArrow | 向右移动跳蚤 |

## 修改注意事项

1. 键名必须是 Unity 支持的 `KeyCode` 名称，如 `F9`、`UpArrow`、`LeftControl`。
2. 修饰键只能使用 `LeftControl`、`RightControl`、`LeftShift`、`RightShift`、`Alt`。
3. 配置文件修改后无需重启游戏，插件会在下一次按键检测时读取新值。
4. 若配置文件中有语法错误，插件会使用默认值。.

## 常用KeyCode对照清单

| 功能              | KeyCode 名称                                          |
| ----------------- | ----------------------------------------------------- |
| 字母键            | A, B, C … Z                                           |
| 数字键            | Alpha0 … Alpha9                                       |
| 功能键            | F1 … F12                                              |
| 方向键            | UpArrow, DownArrow, LeftArrow, RightArrow             |
| 修饰键            | LeftControl, RightControl, LeftShift, RightShift, Alt |
| 回车 / 删除       | Return, Backspace, Delete                             |
| 空格              | Space                                                 |
| Home / End        | Home, End                                             |
| PageUp / PageDown | PageUp, PageDown                                      |
| Tab               | Tab                                                   |
| Escape            | Escape                                                |
| Insert            | Insert                                                |

