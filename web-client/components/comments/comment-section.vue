<template>
  <!-- * Comment section, this is the place where we wanna pass comments and render them, but not create them,
  * it includes top to bottom, wrapper around all comments components -->
  <div>
    <!-- Injecting comment input component -->
    <!-- * Hook to sendComment event, but just passing comment content -> re-emitting again with content -->
    <comment-input label="Add comment" @send="(content) => $emit('send', content)" />

    <v-divider class="my-5"/>

    <!-- Injecting comment component => particular comment, this is actually reply ? -->
    <!-- Binding :comment which is prop in comment component, to comment that we get from arr of comments -->
    <comment v-for="comment in comments" :comment="comment" :key="comment.id"/>
  </div>
</template>

<script>
  import CommentInput from "./comment-input";
  import Comment from "./comment";

  export default {
    // Component name
    name: "comment-section",

    // Injected components
    components: {Comment, CommentInput},

    // Component props
    props: {
      // Comments
      comments: {
        // This means when we inject this component somewhere, we need to pass it this prop
        required: true,
        type: Array,
      }
    }

  }
</script>

<style scoped>

</style>
