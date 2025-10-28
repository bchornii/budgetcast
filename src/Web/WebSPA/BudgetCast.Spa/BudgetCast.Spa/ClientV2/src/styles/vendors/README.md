# Vendor Library Integrations

This folder contains integrations and overrides for third-party UI libraries to ensure they align with our design system.

## Current Integrations

### Bootstrap 5 (`_bootstrap.scss`)

**Purpose:** Provides utility classes and pre-built components following Bootstrap's grid system and component library.

**Integration Strategy:**
1. Import Bootstrap after design tokens to allow overrides
2. Override Bootstrap components with our CSS custom properties
3. Ensure all components respect our theme system (including dark mode support)

**Usage in Components:**
```html
<!-- Use Bootstrap classes directly -->
<div class="container">
  <div class="row g-3">
    <div class="col-md-6">
      <button class="btn btn-primary">Primary Action</button>
    </div>
  </div>
</div>
```

**Customization:**
- All Bootstrap components use our design tokens (colors, spacing, borders, shadows)
- Components automatically respect theme changes via CSS custom properties
- To further customize, edit `_bootstrap.scss` variable overrides section

**Key Overrides:**
- Buttons use `--color-primary`, `--radius-md`, and `--transition-normal`
- Forms use `--color-border-medium` and `--color-text-primary`
- Cards use `--shadow-sm` and `--radius-lg`
- All components support our theme system

### Angular Material (`_angular-material.scss`)

**Purpose:** Overrides for Angular Material components (if/when used).

**Status:** Template ready for Angular Material integration.

## Adding New Vendor Libraries

When adding a new third-party UI library:

1. **Create a new partial file** (e.g., `_primeng.scss`, `_ng-bootstrap.scss`)

2. **Follow this structure:**
```scss
// Library Name Integration
// Description

// 1. Import design tokens
@use '../abstracts/variables';

// 2. Override library Sass variables (if applicable)
// $library-primary: value;

// 3. Import the library
@import 'library-name/styles';

// 4. Override components with CSS custom properties
.library-component {
  color: var(--color-text-primary);
  background: var(--color-bg-primary);
  // ... more overrides
}
```

3. **Import in `styles.scss`:**
```scss
@use 'styles/vendors/your-library';
```

4. **Document it here** in this README

## Best Practices

1. **Always use CSS custom properties** from our design system for overrides
2. **Test with dark theme** if applicable
3. **Keep vendor code isolated** - don't mix with application styles
4. **Document component overrides** with comments
5. **Provide usage examples** in comments or this README

## Theme Support

All vendor integrations should support our theme system by:
- Using CSS custom properties instead of hardcoded values
- Respecting theme changes without requiring component reinitialization
- Supporting both light and dark themes (if applicable)

## Performance Considerations

- Import only the components you need (when supported by the library)
- Use tree-shakeable imports where possible
- Monitor bundle size impact when adding new libraries
- Consider lazy-loading vendor styles for specific feature modules
