# CI/CD Plan for Pomodoro Timer - GitHub Actions

## Overview
This document outlines the CI/CD strategy for the Pomodoro Timer Blazor WebAssembly application using GitHub Actions. The plan covers automated testing, building, and deployment workflows.

## Current Project Status
- **Framework**: Blazor WebAssembly (.NET 9.0)
- **Project Type**: PWA (Progressive Web App)
- **Testing**: xUnit tests for core services
- **Build Output**: Static files for web hosting
- **Deployment Target**: GitHub Pages (recommended) or other static hosting

## CI/CD Goals

### Primary Objectives
- [x] **Continuous Integration**: Automated testing on every push/PR
- [x] **Automated Building**: Generate production-ready artifacts
- [x] **Automated Deployment**: Deploy to hosting platform on main branch updates
- [x] **Quality Gates**: Ensure code quality and test coverage
- [x] **Multi-environment Support**: Development, staging, and production workflows

### Secondary Objectives
- [ ] **Performance Monitoring**: Lighthouse CI for PWA performance
- [ ] **Security Scanning**: Dependency vulnerability checks
- [ ] **Code Coverage Reports**: Track test coverage over time
- [ ] **Release Automation**: Semantic versioning and release notes

## Workflow Strategy

### 1. CI Workflow (Continuous Integration)
**Trigger**: Pull requests and pushes to any branch
**Purpose**: Validate code quality and functionality

```yaml
# .github/workflows/ci.yml
name: CI
on: [push, pull_request]
```

**Steps**:
1. **Setup Environment**
   - Checkout code
   - Setup .NET 9.0 SDK
   - Restore NuGet packages

2. **Code Quality Checks**
   - Lint/format validation
   - Build verification (no warnings)
   - Static code analysis

3. **Testing**
   - Unit tests execution
   - Test result reporting
   - Code coverage collection

4. **Build Validation**
   - Production build test
   - PWA manifest validation
   - Asset optimization check

### 2. CD Workflow (Continuous Deployment)
**Trigger**: Pushes to main branch (after CI passes)
**Purpose**: Deploy to production environment

```yaml
# .github/workflows/cd.yml
name: CD
on:
  push:
    branches: [main]
```

**Steps**:
1. **Production Build**
   - Optimized Blazor WASM build
   - Asset compression and bundling
   - Service worker generation

2. **Deployment**
   - Deploy to GitHub Pages
   - Update PWA cache
   - Invalidate CDN (if applicable)

3. **Post-Deployment**
   - Smoke tests
   - Performance validation
   - Notification (optional)

### 3. Release Workflow (Optional)
**Trigger**: Git tags (v*.*.*)
**Purpose**: Create versioned releases

```yaml
# .github/workflows/release.yml
name: Release
on:
  push:
    tags: ['v*.*.*']
```

## Implementation Plan

### Phase 1: Basic CI/CD (Priority 1)
```markdown
- [ ] Create `.github/workflows/ci.yml`
- [ ] Create `.github/workflows/cd.yml`
- [ ] Setup GitHub Pages deployment
- [ ] Configure branch protection rules
- [ ] Test workflows with sample commits
```

### Phase 2: Enhanced Quality Gates (Priority 2)
```markdown
- [ ] Add code coverage reporting
- [ ] Implement security scanning
- [ ] Add performance testing
- [ ] Setup status badges in README
- [ ] Configure failure notifications
```

### Phase 3: Advanced Features (Priority 3)
```markdown
- [ ] Multi-environment deployments (staging/prod)
- [ ] Release automation with semantic versioning
- [ ] Lighthouse CI for PWA performance
- [ ] Dependency update automation (Dependabot)
- [ ] Docker containerization (optional)
```

## Workflow Files Structure

```
.github/
├── workflows/
│   ├── ci.yml                 # Continuous Integration
│   ├── cd.yml                 # Continuous Deployment
│   └── release.yml            # Release Management (optional)
├── dependabot.yml             # Dependency updates
└── CODEOWNERS                 # Code review assignments
```

## Deployment Targets

### Recommended: GitHub Pages - Detailed Process

#### Why GitHub Pages for Blazor WASM?
- **Perfect Match**: Blazor WASM compiles to static files (HTML, CSS, JS, WASM) that GitHub Pages serves directly
- **Zero Server Requirements**: No need for .NET runtime on the server
- **Free Hosting**: Unlimited bandwidth for public repositories
- **HTTPS by Default**: Automatic SSL certificates
- **Custom Domains**: Support for custom domain names
- **CDN Benefits**: Global content delivery network

#### How Blazor WASM Works with GitHub Pages
```
Your C# Code → .NET Compiler → WebAssembly + JS + HTML → Static Files → GitHub Pages
```

1. **Build Process**: `dotnet publish` generates static assets in `wwwroot`
2. **File Structure**: All files are static - no server-side processing needed
3. **Routing**: Blazor handles client-side routing via JavaScript
4. **PWA Features**: Service worker and manifest work perfectly with static hosting

#### Step-by-Step GitHub Pages Setup

##### 1. Repository Configuration
```markdown
Navigate to: Your GitHub Repository → Settings → Pages

Configuration:
- Source: "Deploy from a branch" OR "GitHub Actions" (recommended)
- Branch: None initially (we'll use Actions)
- Custom domain: Optional (yourdomain.com)
```

##### 2. GitHub Actions Deployment Method (Recommended)
This method gives you full control over the build and deployment process:

**Advantages:**
- Custom build steps (optimizations, compression)
- Environment variables control
- Build validation before deployment
- Multiple environment support

**Workflow Process:**
```yaml
# .github/workflows/deploy.yml
1. Trigger: Push to main branch
2. Setup: Install .NET 9.0 SDK
3. Build: dotnet publish --configuration Release
4. Optimize: Compress assets, validate PWA manifest
5. Deploy: Upload to GitHub Pages
6. Verify: Test deployment success
```

##### 3. Manual Branch Deployment (Alternative)
Simpler but less flexible:
- Create `gh-pages` branch
- Manually build and push static files
- GitHub serves directly from branch

#### Detailed Deployment Workflow

##### Pre-Deployment: Blazor WASM Build Process
```bash
# What happens during build:
dotnet publish -c Release -o publish

# Generated files structure:
publish/wwwroot/
├── index.html                 # App entry point
├── manifest.json             # PWA manifest
├── service-worker.js         # PWA service worker
├── _framework/              # Blazor runtime files
│   ├── blazor.webassembly.js
│   ├── PomodoroTimer.wasm   # Your compiled C# code
│   └── dotnet.*.wasm        # .NET runtime
├── css/                     # Stylesheets
├── js/                      # JavaScript files
└── _content/               # Static content
```

##### Deployment Steps in Detail

**Step 1: Environment Setup**
```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '9.0.x'
    
- name: Checkout code
  uses: actions/checkout@v4
```

**Step 2: Dependency Management**
```yaml
- name: Restore dependencies
  run: dotnet restore
  
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
```

**Step 3: Build and Publish**
```yaml
- name: Build and publish
  run: |
    dotnet publish -c Release -o dist/
    
    # Blazor WASM specific optimizations
    # - Compression enabled
    # - Trim unused code
    # - Optimize for size
```

**Step 4: PWA Optimization**
```yaml
- name: Optimize for PWA
  run: |
    # Validate manifest.json
    # Ensure service worker is properly configured
    # Check icon sizes and formats
    # Verify offline functionality
```

**Step 5: GitHub Pages Deployment**
```yaml
- name: Deploy to GitHub Pages
  uses: peaceiris/actions-gh-pages@v3
  with:
    github_token: ${{ secrets.GITHUB_TOKEN }}
    publish_dir: ./dist/wwwroot
    # This uploads the static files to gh-pages branch
```

#### GitHub Pages Configuration Details

##### Repository Settings
```markdown
1. Go to: Settings → Pages
2. Select Source: "GitHub Actions" 
3. No branch selection needed (Actions handles it)
4. Custom domain: Optional setup
5. Enforce HTTPS: Enabled (recommended)
```

##### URL Structure
```markdown
Default URL: https://yourusername.github.io/PomodoroTimer
Custom domain: https://yourdomain.com (if configured)

Path structure:
/ → index.html (Blazor app entry point)
/_framework/ → Blazor runtime files
/css/ → Stylesheets
/manifest.json → PWA manifest
/service-worker.js → PWA service worker
```

#### PWA Specific Considerations

##### Service Worker Deployment
```javascript
// service-worker.js considerations for GitHub Pages:
const CACHE_NAME = 'pomodoro-timer-v1.0.0';
const BASE_URL = '/PomodoroTimer/'; // Important: include repo name for GitHub Pages

// Cache strategy for GitHub Pages
const urlsToCache = [
  `${BASE_URL}`,
  `${BASE_URL}index.html`,
  `${BASE_URL}_framework/blazor.webassembly.js`,
  // ... other assets
];
```

##### Manifest.json Configuration
```json
{
  "name": "Pomodoro Timer",
  "short_name": "Pomodoro",
  "start_url": "/PomodoroTimer/",  // Must include repo name
  "scope": "/PomodoroTimer/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#667eea"
}
```

#### Routing Configuration for GitHub Pages

##### Client-Side Routing Setup
Blazor WASM uses client-side routing, but GitHub Pages needs special configuration:

**Option 1: Hash Routing (Simplest)**
```csharp
// In Program.cs
builder.Services.AddSingleton<NavigationManager, HashNavigationManager>();
```

**Option 2: 404.html Fallback (Better UX)**
Create `404.html` that redirects to `index.html`:
```html
<!-- 404.html -->
<!DOCTYPE html>
<html>
<head>
    <script>
        // Redirect to index.html with path as query parameter
        var path = window.location.pathname.replace('/PomodoroTimer', '');
        window.location.replace('/PomodoroTimer/' + (path === '/' ? '' : '?redirect=' + path));
    </script>
</head>
<body>Redirecting...</body>
</html>
```

#### Performance Optimizations

##### Build Optimizations
```xml
<!-- In PomodoroTimer.csproj -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <PublishTrimmed>true</PublishTrimmed>
  <BlazorEnableCompression>true</BlazorEnableCompression>
  <BlazorWebAssemblyPreserveCollationData>false</BlazorWebAssemblyPreserveCollationData>
</PropertyGroup>
```

##### GitHub Actions Optimizations
```yaml
- name: Compress assets
  run: |
    # Gzip static files for better loading performance
    find dist/wwwroot -type f \( -name "*.js" -o -name "*.css" -o -name "*.html" \) -exec gzip -k {} \;
```

#### Monitoring and Validation

##### Post-Deployment Checks
```yaml
- name: Validate deployment
  run: |
    # Check if site is accessible
    curl -f https://yourusername.github.io/PomodoroTimer/
    
    # Validate PWA manifest
    curl -f https://yourusername.github.io/PomodoroTimer/manifest.json
    
    # Check service worker
    curl -f https://yourusername.github.io/PomodoroTimer/service-worker.js
```

##### Performance Monitoring
```yaml
- name: Lighthouse CI
  uses: treosh/lighthouse-ci-action@v9
  with:
    urls: |
      https://yourusername.github.io/PomodoroTimer/
    configPath: './lighthouserc.json'
```

#### Troubleshooting Common Issues

##### Issue 1: 404 Errors on Refresh
**Problem**: Direct navigation to routes returns 404
**Solution**: Implement 404.html fallback or use hash routing

##### Issue 2: Assets Not Loading
**Problem**: CSS/JS files return 404
**Solution**: Check base href in index.html matches repository name

##### Issue 3: PWA Not Installing
**Problem**: "Add to Home Screen" not appearing
**Solution**: Verify HTTPS, valid manifest, and service worker registration

##### Issue 4: Slow Loading
**Problem**: Large initial download size
**Solution**: Enable compression, trim unused code, lazy loading

#### Security Considerations

##### Content Security Policy
```html
<!-- In index.html -->
<meta http-equiv="Content-Security-Policy" 
      content="default-src 'self'; 
               script-src 'self' 'unsafe-eval'; 
               style-src 'self' 'unsafe-inline';">
```

##### HTTPS Enforcement
GitHub Pages automatically provides HTTPS, but ensure:
- All resource links use HTTPS
- Service worker requires HTTPS
- PWA features need secure context

#### Cost and Limitations

##### GitHub Pages Limits
- **Bandwidth**: 100GB/month (soft limit)
- **Storage**: 1GB repository size limit
- **Build Time**: 10 minutes maximum
- **Frequency**: 10 builds per hour maximum

##### Blazor WASM Considerations
- **First Load**: Larger than typical SPAs due to .NET runtime
- **Caching**: Aggressive caching strategy needed
- **SEO**: Limited without pre-rendering (consider Blazor Server for SEO-critical apps)

### Alternative Options
1. **Netlify**: Easy setup, preview deployments, custom domains
2. **Vercel**: Optimized for frontend, excellent performance
3. **Azure Static Web Apps**: Microsoft ecosystem integration
4. **AWS S3 + CloudFront**: Enterprise-grade, highly scalable

## Security Considerations

### Secrets Management
```markdown
- `GITHUB_TOKEN`: Automatic (for GitHub Pages)
- `DEPLOY_TOKEN`: For external deployments
- `LIGHTHOUSE_API_KEY`: For performance monitoring
```

### Branch Protection
```markdown
- Require PR reviews before merging to main
- Require status checks to pass
- Restrict push to main branch
- Enable "Require up-to-date branches"
```

## Monitoring & Notifications

### Success Metrics
- **Build Success Rate**: Target >98%
- **Test Coverage**: Maintain >80%
- **Deployment Time**: <5 minutes end-to-end
- **PWA Performance Score**: >90 (Lighthouse)

### Failure Handling
- **Failed Builds**: Email notifications to maintainers
- **Test Failures**: Block deployment automatically
- **Deployment Issues**: Automatic rollback capability

## Configuration Files

### GitHub Actions Secrets Needed
```markdown
- GITHUB_TOKEN (auto-provided)
- PERSONAL_ACCESS_TOKEN (for cross-repo operations)
- DEPLOY_TOKEN (if using external hosting)
```

### Repository Settings
```markdown
- Enable GitHub Pages (source: GitHub Actions)
- Configure branch protection for main
- Enable vulnerability alerts
- Set up Dependabot for security updates
```

## Expected Benefits

### For Development
- **Faster Feedback**: Know within minutes if changes break anything
- **Quality Assurance**: Automated testing prevents regressions
- **Consistent Builds**: Same process every time, no manual errors

### For Deployment
- **Zero-Downtime**: Automated deployments with validation
- **Quick Rollbacks**: Easy to revert to previous versions
- **Environment Consistency**: Same build process for all environments

### For Maintenance
- **Dependency Updates**: Automated security patches
- **Performance Monitoring**: Track app performance over time
- **Release Management**: Structured versioning and release notes

## Timeline

### Week 1: Foundation
- Setup basic CI workflow
- Configure GitHub Pages deployment
- Test with sample changes

### Week 2: Enhancement
- Add comprehensive testing
- Implement code coverage
- Setup monitoring and notifications

### Week 3: Polish
- Add performance testing
- Configure security scanning
- Document process and troubleshooting

## Success Criteria

### Must Have ✅
- [x] Automated testing on every commit
- [x] Automated deployment to production
- [x] Build failure notifications
- [x] Branch protection for main

### Should Have 🎯
- [ ] Code coverage reporting
- [ ] Performance monitoring
- [ ] Security vulnerability scanning
- [ ] Multi-environment support

### Nice to Have 🌟
- [ ] Lighthouse CI for PWA scoring
- [ ] Automated dependency updates
- [ ] Release note generation
- [ ] Slack/Discord notifications

## Risk Mitigation

### Potential Issues
1. **Build Dependencies**: .NET SDK availability and versions
2. **Deployment Permissions**: GitHub Pages configuration
3. **Test Flakiness**: Blazor component testing challenges
4. **Resource Limits**: GitHub Actions runner limitations

### Mitigation Strategies
1. **Pin SDK Versions**: Specify exact .NET version in workflows
2. **Test Locally**: Use Act to test GitHub Actions locally
3. **Retry Logic**: Add automatic retry for flaky tests
4. **Monitoring**: Set up alerts for workflow failures

## Complete Implementation Guide

### Phase 1: Basic GitHub Pages Setup (30 minutes)

#### Step-by-Step Checklist

##### 1. Prepare Your Repository (5 minutes)
```markdown
- [ ] Push your code to GitHub repository
- [ ] Ensure main branch has latest changes
- [ ] Verify project builds locally with `dotnet publish -c Release`
```

##### 2. Configure Repository Settings (5 minutes)
```markdown
- [ ] Go to GitHub repository → Settings → Pages
- [ ] Source: Select "GitHub Actions"
- [ ] Save settings (GitHub will show deployment instructions)
```

##### 3. Create Workflow Directory (2 minutes)
```bash
mkdir -p .github/workflows
```

##### 4. Create CI/CD Workflow File (15 minutes)
Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to GitHub Pages

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --configuration Release --no-restore
      
    - name: Test
      run: dotnet test --no-build --verbosity normal
      
    - name: Publish
      run: dotnet publish -c Release -o dist/
      
    - name: Upload Pages Artifact
      if: github.ref == 'refs/heads/main'
      uses: actions/upload-pages-artifact@v2
      with:
        path: dist/wwwroot/

  deploy:
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    needs: build-and-test
    
    permissions:
      pages: write
      id-token: write
      
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
      
    steps:
    - name: Deploy to GitHub Pages
      id: deployment
      uses: actions/deploy-pages@v2
```

##### 5. Configure Base Path for GitHub Pages (3 minutes)
Update `wwwroot/index.html`:
```html
<!-- Change this line -->
<base href="/" />
<!-- To this (replace 'PomodoroTimer' with your repo name) -->
<base href="/PomodoroTimer/" />
```

##### 6. Test Deployment (5 minutes)
```markdown
- [ ] Commit and push the workflow file
- [ ] Go to Actions tab to watch build progress
- [ ] Check deployment at https://yourusername.github.io/PomodoroTimer
- [ ] Test PWA installation on mobile device
```

### Phase 2: PWA Optimization (45 minutes)

#### Fix PWA Paths for GitHub Pages

##### Update Service Worker (15 minutes)
Modify `wwwroot/service-worker.js`:

```javascript
// Add base path for GitHub Pages
const baseHref = document.getElementsByTagName('base')[0]?.href ?? '/';
const cacheUrls = [
    baseHref,
    `${baseHref}css/app.css`,
    `${baseHref}icon-192.svg`,
    `${baseHref}manifest.json`
    // Add other assets
];
```

##### Update PWA Manifest (10 minutes)
Modify `wwwroot/manifest.json`:

```json
{
  "name": "Pomodoro Timer",
  "short_name": "Pomodoro",
  "start_url": "/PomodoroTimer/",
  "scope": "/PomodoroTimer/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#667eea",
  "icons": [
    {
      "src": "/PomodoroTimer/icon-192.svg",
      "sizes": "192x192",
      "type": "image/svg+xml"
    }
  ]
}
```

##### Add 404 Fallback for Client Routing (10 minutes)
Create `wwwroot/404.html`:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Pomodoro Timer</title>
    <script>
        // GitHub Pages SPA fallback
        var repo = '/PomodoroTimer';
        var l = window.location;
        l.replace(l.protocol + '//' + l.hostname + (l.port ? ':' + l.port : '') + 
                 repo + '/#' + l.pathname.slice(repo.length) + l.search);
    </script>
</head>
<body>Redirecting...</body>
</html>
```

##### Enhanced Workflow with PWA Validation (10 minutes)
Update `.github/workflows/deploy.yml` to add PWA checks:

```yaml
    - name: Validate PWA Configuration
      run: |
        # Check manifest.json is valid
        python -m json.tool dist/wwwroot/manifest.json
        
        # Verify service worker exists
        test -f dist/wwwroot/service-worker.js
        
        # Check base href is set correctly
        grep -q 'base href="/PomodoroTimer/"' dist/wwwroot/index.html
```

### Phase 3: Advanced Features (60 minutes)

#### Add Performance Monitoring (20 minutes)

##### Lighthouse CI Setup
Create `.github/workflows/lighthouse.yml`:

```yaml
name: Lighthouse CI

on:
  deployment_status:

jobs:
  lighthouse:
    if: github.event.deployment_status.state == 'success'
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      
    - name: Run Lighthouse CI
      uses: treosh/lighthouse-ci-action@v9
      with:
        urls: |
          ${{ github.event.deployment_status.target_url }}
        configPath: './lighthouserc.json'
        uploadArtifacts: true
```

Create `lighthouserc.json`:
```json
{
  "ci": {
    "collect": {
      "numberOfRuns": 3
    },
    "assert": {
      "assertions": {
        "categories:performance": ["warn", {"minScore": 0.8}],
        "categories:accessibility": ["error", {"minScore": 0.9}],
        "categories:best-practices": ["warn", {"minScore": 0.9}],
        "categories:seo": ["warn", {"minScore": 0.8}],
        "categories:pwa": ["error", {"minScore": 0.9}]
      }
    }
  }
}
```

#### Add Security Scanning (20 minutes)

##### Dependabot Configuration
Create `.github/dependabot.yml`:

```yaml
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 5
    
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
```

##### Security Workflow
Create `.github/workflows/security.yml`:

```yaml
name: Security Scan

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
  schedule:
    - cron: '0 6 * * 1'  # Weekly on Monday

jobs:
  security:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout
      uses: actions/checkout@v4
      
    - name: Run Trivy vulnerability scanner
      uses: aquasecurity/trivy-action@master
      with:
        scan-type: 'fs'
        scan-ref: '.'
        format: 'sarif'
        output: 'trivy-results.sarif'
        
    - name: Upload Trivy scan results
      uses: github/codeql-action/upload-sarif@v2
      if: always()
      with:
        sarif_file: 'trivy-results.sarif'
```

#### Add Code Coverage (20 minutes)

##### Coverage Workflow
Update your test job in `deploy.yml`:

```yaml
    - name: Test with Coverage
      run: dotnet test --configuration Release --logger trx --collect:"XPlat Code Coverage"
      
    - name: Code Coverage Report
      uses: irongut/CodeCoverageSummary@v1.3.0
      with:
        filename: '**/coverage.cobertura.xml'
        badge: true
        fail_below_min: true
        format: markdown
        hide_branch_rate: false
        hide_complexity: true
        indicators: true
        output: both
        thresholds: '60 80'
        
    - name: Add Coverage PR Comment
      uses: marocchino/sticky-pull-request-comment@v2
      if: github.event_name == 'pull_request'
      with:
        recreate: true
        path: code-coverage-results.md
```

### Expected Results

#### After Phase 1 Completion:
- ✅ **Live URL**: https://yourusername.github.io/PomodoroTimer
- ✅ **Automatic Deployment**: Every push to main triggers deployment
- ✅ **PWA Installation**: App can be installed on mobile devices
- ✅ **Build Validation**: Tests must pass before deployment

#### After Phase 2 Completion:
- ✅ **Proper Routing**: Direct URLs work correctly
- ✅ **Offline Support**: App works without internet connection
- ✅ **Performance**: Optimized loading and caching

#### After Phase 3 Completion:
- ✅ **Performance Monitoring**: Lighthouse scores tracked
- ✅ **Security Scanning**: Automated vulnerability detection
- ✅ **Code Coverage**: Test coverage reporting and tracking

### Troubleshooting Guide

#### Common Issues and Solutions

##### Build Failures
```bash
# Check logs in GitHub Actions tab
# Common fixes:
- Ensure .NET 9.0 SDK is specified correctly
- Verify all NuGet packages restore successfully
- Check for compilation errors in code
```

##### 404 Errors After Deployment
```bash
# Check base href in index.html
# Verify repository name matches base path
# Ensure 404.html fallback is in place
```

##### PWA Not Installing
```bash
# Verify HTTPS (GitHub Pages provides this automatically)
# Check manifest.json is valid JSON
# Ensure service worker is registered correctly
# Test on mobile device Chrome/Safari
```

##### Slow Loading Performance
```bash
# Enable compression in workflow
# Implement lazy loading for large components
# Optimize images and assets
# Use CDN for external resources
```

### Next Steps

1. **Create CI workflow file** - Start with basic build and test
2. **Setup GitHub Pages** - Configure repository for static hosting
3. **Test deployment** - Verify full CI/CD pipeline works
4. **Add enhancements** - Gradually add more features
5. **Document process** - Create runbook for troubleshooting

---

*This plan will evolve as we implement each phase. Priority should be on getting basic CI/CD working first, then enhancing with additional features.*
