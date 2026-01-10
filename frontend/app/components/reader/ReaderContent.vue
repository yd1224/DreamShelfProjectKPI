<script setup lang="ts">
interface Props {
  chapterNumber: number
  chapterTitle: string
  htmlContent: string
}

const props = defineProps<Props>()

const text = props.htmlContent.replaceAll('[PATH]', useRuntimeConfig().public.apiBaseUrl+'/images/')
</script>

<template>
  <article class="reader-content">
    <header class="chapter-header">
      <span class="chapter-number">Chapter {{ chapterNumber }}</span>
      <h2 class="chapter-title">{{ chapterTitle }}</h2>
    </header>
    
    <div class="chapter-body" v-html="text  " />
  </article>
</template>

<style scoped lang="scss">
@mixin mobile {
  @media (max-width: 767px) {
    @content;
  }
}

@mixin tablet {
  @media (min-width: 768px) {
    @content;
  }
}

@mixin desktop {
  @media (min-width: 992px) {
    @content;
  }
}

.reader-content {
  max-width: 720px;
  margin: 0 auto;
  padding: 2rem 1rem;

  @include tablet {
    padding: 2.5rem 1.5rem;
  }

  @include desktop {
    padding: 3rem 2rem;
  }
}

.chapter-header {
  text-align: center;
  margin-bottom: 2.5rem;
  padding-bottom: 2rem;
  border-bottom: 1px solid rgba(75, 85, 99, 0.3);
  position: relative;

  &::after {
    content: '';
    position: absolute;
    bottom: -1px;
    left: 50%;
    transform: translateX(-50%);
    width: 80px;
    height: 3px;
    background: linear-gradient(90deg, transparent, #3b82f6, transparent);
    border-radius: 2px;
  }

  @include tablet {
    margin-bottom: 3rem;
    padding-bottom: 2.5rem;
  }
}

.chapter-number {
  display: inline-block;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #3b82f6;
  text-transform: uppercase;
  letter-spacing: 0.15em;
  margin-bottom: 0.75rem;
  padding: 0.375rem 1rem;
  background: rgba(59, 130, 246, 0.1);
  border-radius: 1rem;
  border: 1px solid rgba(59, 130, 246, 0.2);

  @include tablet {
    font-size: 0.875rem;
    padding: 0.5rem 1.25rem;
  }
}

.chapter-title {
  font-size: 1.5rem;
  font-weight: 800;
  color: #f9fafb;
  margin: 0;
  line-height: 1.3;
  letter-spacing: -0.02em;

  @include tablet {
    font-size: 2rem;
  }

  @include desktop {
    font-size: 2.25rem;
  }
}

.chapter-body {
  color: #e5e7eb;
  font-size: 1.0625rem;
  line-height: 1.85;
  letter-spacing: 0.01em;

  @include tablet {
    font-size: 1.125rem;
    line-height: 1.9;
  }

  // Deep styles for HTML content
  :deep(p) {
    margin: 0 0 1.5rem 0;

    &:last-child {
      margin-bottom: 0;
    }
  }

  :deep(h1),
  :deep(h2),
  :deep(h3),
  :deep(h4),
  :deep(h5),
  :deep(h6) {
    color: #f9fafb;
    font-weight: 700;
    margin: 2rem 0 1rem 0;
    line-height: 1.3;

    &:first-child {
      margin-top: 0;
    }
  }

  :deep(h1) {
    font-size: 1.75rem;
    @include tablet {
      font-size: 2rem;
    }
  }

  :deep(h2) {
    font-size: 1.5rem;
    @include tablet {
      font-size: 1.75rem;
    }
  }

  :deep(h3) {
    font-size: 1.25rem;
    @include tablet {
      font-size: 1.5rem;
    }
  }

  :deep(strong),
  :deep(b) {
    color: #f9fafb;
    font-weight: 600;
  }

  :deep(em),
  :deep(i) {
    font-style: italic;
  }

  :deep(a) {
    color: #60a5fa;
    text-decoration: underline;
    text-underline-offset: 2px;
    transition: color 0.2s ease;

    &:hover {
      color: #93c5fd;
    }
  }

  :deep(blockquote) {
    margin: 1.5rem 0;
    padding: 1rem 1.5rem;
    border-left: 4px solid #3b82f6;
    background: rgba(59, 130, 246, 0.08);
    border-radius: 0 8px 8px 0;
    font-style: italic;
    color: #d1d5db;

    p {
      margin: 0;
    }
  }

  :deep(ul),
  :deep(ol) {
    margin: 1.5rem 0;
    padding-left: 1.5rem;

    li {
      margin-bottom: 0.5rem;

      &:last-child {
        margin-bottom: 0;
      }
    }
  }

  :deep(hr) {
    border: none;
    height: 1px;
    background: linear-gradient(90deg, transparent, rgba(75, 85, 99, 0.5), transparent);
    margin: 2.5rem 0;
  }

  :deep(code) {
    font-family: 'SF Mono', 'Fira Code', monospace;
    font-size: 0.9em;
    padding: 0.2em 0.4em;
    background: rgba(75, 85, 99, 0.3);
    border-radius: 4px;
    color: #fbbf24;
  }

  :deep(pre) {
    margin: 1.5rem 0;
    padding: 1rem 1.25rem;
    background: rgba(31, 41, 55, 0.8);
    border-radius: 8px;
    overflow-x: auto;
    border: 1px solid rgba(75, 85, 99, 0.3);

    code {
      padding: 0;
      background: none;
    }
  }

  :deep(img) {
    max-width: 100%;
    height: auto;
    border-radius: 8px;
    margin: 1.5rem 0;
  }
}
</style>
