<template>
  <!-- * Comment section, this is the place where we wanna pass comments and render them, but not create them,
  * it includes top to bottom, wrapper around all comments components -->
  <div>
    <!-- Injecting comment input component -->
    <!-- * Hook to sendComment event, but just passing comment content -> re-emitting again with content -->
    <comment-input label="comment"
                   :parent-id="parentId"
                   :parent-type="parentType"
                   @comment-created="prependComment" />

    <v-divider class="my-5"/>

    <!-- Injecting comment component => particular comment, this is actually reply ? -->
    <!-- Binding :comment which is prop in comment component, to comment that we get from arr of comments -->
    <comment v-for="comment in comments" :comment="comment" :key="comment.id"/>
  </div>
</template>

<script>
  import CommentInput from "./comment-input";
  import Comment from "./comment";
  import {COMMENT_PARENT_TYPE, configurable, container} from "@/components/comments/_shared";

  export default {
    // Component name
    name: "comment-section",

    // Injected components
    components: {Comment, CommentInput},

    mixins: [configurable, container],

    created() {
      return this.$axios.$get(this.endpoint)
        .then((comments) => comments.forEach(comment => this.comments.push(comment)))
    },

    computed: {
      endpoint() {
        if (this.parentType === COMMENT_PARENT_TYPE.MODERATION_ITEM) {
          return `/api/moderation-items/${this.parentId}/comments`
        }

        if (this.parentType === COMMENT_PARENT_TYPE.SUBMISSION) {
          return `/api/submissions/${this.parentId}/comments`
        }

      }
    }
  }
</script>

<style scoped>

</style>
