VNode Changelog
[v0.1] – Initial Alpha

Date: 2025-10-13

Added

Base Node and NodePort classes with generic value support.

NodeTransform for canvas-positioned nodes.

NodeCollector to hold and manage nodes.

Basic editor window (VNodeWindow and NodeEditorWindow) with:

Node creation via context menu.

Dragging nodes with left mouse.

Connecting nodes through ports.

Scrollable node canvas.

Node selection and deletion.

Basic input/output port drawing and value fields.

Grid background for node canvas.

ReadMe viewer integrated into editor window.

NodeStyling class to define colors for nodes, ports, grid, and background.

Changed

Replaced hard-coded colors with configurable NodeStyling colors.

Port and node positions now use canvas space, supporting scrolling and panning.

Connection lines use Bezier curves.

Scroll view integrated into editor window for large node canvases.

Fixed

Node selection and dragging now respect canvas scrolling.

Port hit detection corrected for canvas coordinates.

Added generic method to create nodes dynamically from any Node subclass.

Known Issues

Connection lines drawn with Handles instead of GUI may not perfectly clip within scroll view.

Multi-selection not yet implemented.

Node saving/loading not yet implemented.

Context menu does not categorize node types yet.

When dragging node that has a port, the port feels choppy moved.

Upcoming Features

Node serialization: positions, connections, and values.

Categorized context menu.

Multi-selection support.

Node styling presets and theming.