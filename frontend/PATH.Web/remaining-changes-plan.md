# PATH.Web Remaining Changes Plan

A concise roadmap of what is still left from the implementation plan, organized by priority.

## Must Do

### 1. Authorization Consistency
- Normalize role checks everywhere so `Admin`, `Manager`, and `admin` are handled in one consistent style.
- Align the rules across:
  - `src/app/auth/auth.service.ts`
  - `src/app/auth/role.guard.ts`
  - `src/app/shared/layout/layout.component.html`
  - `src/app/organization/organization.component.ts`
  - `src/app/project/project.component.ts`
- Keep the same access rules in both templates and route logic so visibility and enforcement match.

### 2. Route Protection
- Apply `roleGuard` where the app actually needs route-level protection in `src/app/app.routes.ts`.
- Add admin-only or manager-only route checks instead of relying only on hidden buttons.
- Add a fallback route or 404 screen for unknown paths.

### 3. Remove Remaining Console Noise
- Delete leftover `console.log` calls from the component/service layer.
- Priority files:
  - `src/app/project/project.component.ts`
  - `src/app/services/project.service.ts`
- Keep user-facing errors in the UI, not in the browser console.

## Nice to Have

### 4. Shared UI Extraction
- Turn the delete confirmation modal into a reusable shared component.
- Later, reuse the same pattern for alerts, empty states, and form-row layouts.
- This will make the UI feel more consistent and reduce duplicated markup.

### 5. Architecture Cleanup
- Evolve the app toward a clearer `core / shared / features` structure.
- Suggested direction:
  - `core` for guards, interceptors, auth, and layout shell concerns
  - `shared` for reusable UI and helpers
  - `features` or `domains` for organizations, projects, tasks, and related screens
- Keep standalone components, but group them by domain instead of scattering them across top-level folders.

### 6. UX Consistency
- Standardize loading, empty, and failure states across all main screens.
- Make spacing, form controls, and action buttons follow one visual system.
- Keep the design compact and intentional rather than mixing several styles.

## Optional, But Good for CV Value

### 7. Test and CI Hardening
- Keep the test suite maintained if you want the repo to feel production-grade.
- Add or tighten tests around auth, guards, route access, and critical flows.
- Add CI checks for build, test, and lint so the project feels complete and trustworthy.

### 8. Portfolio Polish
- Add a 404 page and better route fallback behavior.
- Improve small UI details like breadcrumb/back navigation, section spacing, and card alignment.
- Build a small reusable component library over time if you want the codebase to look more mature.

## Recommended Order
1. Authorization consistency
2. Route protection
3. Remove console noise
4. Shared UI extraction
5. Architecture cleanup
6. UX consistency
7. Test and CI hardening
8. Portfolio polish

## Short Version
If you only want the highest-impact items, do these first:
- Fix route and role consistency.
- Add real route protection.
- Clean up noisy logs.
- Extract reusable modal/shared UI pieces.
- Reorganize toward `core / shared / features`.
