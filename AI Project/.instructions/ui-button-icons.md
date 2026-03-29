# UI Elements: Button Icon Requirement

## Rule
All buttons in the user interface must have a corresponding icon that visually represents the button's action or purpose. This applies to all HTML and JavaScript-generated buttons in the project.

## Scope
- Applies to all files in the project, including HTML and JavaScript files.
- Applies to all button elements, regardless of their function (e.g., submit, download, navigation).

## Rationale
Icons improve usability and accessibility by providing visual cues, making the interface more intuitive and user-friendly.

## Example
**Before:**
```html
<button id="downloadChart">Download Chart</button>
```

**After:**
```html
<button id="downloadChart"><i class="bi bi-download"></i> Download Chart</button>
```

## Enforcement
- When creating or editing a button, always include an appropriate icon (e.g., using Bootstrap Icons or similar libraries).
- If a button's action is not easily represented by a standard icon, use a generic action icon (e.g., <i class="bi bi-gear"></i> for settings).

## Related Customizations
- Enforce icon usage for other interactive elements (e.g., links, dropdowns).
- Standardize icon library (e.g., always use Bootstrap Icons).

## Example Prompts
- "Add a button to export data, with an icon."
- "Update all buttons to include icons."
- "Create a settings button with a gear icon."

---
This instruction enforces consistent use of icons for all buttons, improving the UI's clarity and professionalism.