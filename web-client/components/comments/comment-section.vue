<template>
  <!-- * Comment section, this is the place where we wanna pass comments and render them, but not create them,
  * it includes top to bottom, wrapper around all comments components -->
  <div>
    <!-- Injecting comment input component -->
    <!-- * Hook to sendComment event, but just passing comment content -> re-emitting again with content -->
    <comment-input label="comment"
                   :parent-id="parentId"
                   :parent-type="parentType"
                   @comment-created="(c) => content.unshift(c)" />

    <v-divider class="my-5"/>

    <!-- Injecting comment component => particular comment, this is actually reply ? -->
    <!-- Binding :comment which is prop in comment component, to comment that we get from arr of comments -->
    <comment v-for="comment in content" :comment="comment" :key="comment.id"/>

    <div class="d-flex justify-center" v-if="!finished">
      <v-btn outlined small @click="loadContent">Load More</v-btn>
    </div>
  </div>
</template>

<script>
  import CommentInput from "./comment-input";
  import Comment from "./comment";
  import {configurable} from "@/components/comments/_shared";
  import {feed} from "@/components/feed";

  export default {
    // Component name
    name: "comment-section",

    // Injected components
    components: {Comment, CommentInput},

    mixins: [configurable, feed('first')],

    created() {
      this.loadContent()
    },

    methods: {
      getContentUrl() {
        return `/api/comments/${this.parentId}/${this.parentType}${this.query}`
      }
    }
  }
</script>

<style scoped>

</style>
