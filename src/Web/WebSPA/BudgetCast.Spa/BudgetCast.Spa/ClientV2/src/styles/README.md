# SCSS Architecture - Angular 20 Modern Approach

## Philosophy

This project uses a **component-first styling approach** that leverages Angular's component encapsulation while maintaining a minimal, well-organized global style system.

## Structure

```
src/
├── styles/
│   ├── abstracts/
│   │   ├── _variables.scss      # CSS custom properties, Sass variables
│   │   ├── _functions.scss      # Sass functions
│   │   └── _mixins.scss         # Reusable mixins
│   ├── base/
│   │   ├── _reset.scss          # CSS reset/normalize
│   │   ├── _typography.scss     # Global typography
│   │   └── _utilities.scss      # Utility classes
│   ├── themes/
│   │   ├── _default.scss        # Default theme variables
│   │   └── _dark.scss           # Dark theme (if needed)
│   └── vendors/
│       └── _angular-material.scss # Third-party overrides
└── styles.scss                  # Main global entry point
```

## Key Principles

1. **Component Scoped**: Most styles live in component `.scss` files
2. **Minimal Global**: Only truly global styles in the main system
3. **Design Tokens**: Use CSS custom properties for design system values
4. **Utility First**: Provide utility classes for common patterns
5. **Theme Ready**: Structure supports multiple themes

## Usage Guidelines

### Global Styles (styles.scss)
- CSS resets and normalizations
- Typography foundations
- CSS custom properties (design tokens)
- Utility classes
- Third-party library overrides

### Component Styles
- Component-specific styling
- Use CSS custom properties from global system
- Import mixins/functions as needed via `@use`

### Design Tokens
Use CSS custom properties for:
- Colors
- Spacing
- Typography scale
- Border radius
- Shadows
- Z-index values

### Example Component Usage
```scss
// In component.scss
@use '../../../styles/abstracts/mixins' as mixins;

.component {
  // Use design tokens
  color: var(--color-primary);
  padding: var(--spacing-md);
  
  // Use mixins
  @include mixins.button-base;
}
```

## Migration from 7-1 Pattern

If migrating from 7-1:
1. Move component-specific styles to component files
2. Keep only truly global styles in the global system
3. Convert Sass variables to CSS custom properties
4. Consolidate similar folders (layout → base)
5. Remove page-specific global styles