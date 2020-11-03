<template>
  <div>
    <!-- 1# Binding from comment-body component :comment prop to our comment prop, emitting sendComment event -> both sending to same func -->
    <!-- Replaying to top comment, @load-replies event => loads replies -->
    <comment-body :comment="comment"
                  :parent-id="comment.id"
                  :parent-type="commentParentType"
                  @comment-created="appendComment"
                  @load-replies="loadReplies"/>

    <!-- 2# -->
    <div class="ml-5">
      <comment-body v-for="comment in comments"
                    :comment="comment"
                    :parent-id="comment.id"
                    :parent-type="commentParentType"
                    @comment-created="appendComment"
                    :key="`reply-${comment.id}`"/>
    </div>
  </div>
</template>

<script>
  import CommentBody from "./comment-body";
  import {COMMENT_PARENT_TYPE, container} from "@/components/comments/_shared";

  export default {
    // Component name
    name: "comment",

    // Injected components
    components: {CommentBody},

    mixins: [container],

    // Component props => this is where comment info is stored
    props: {
      // Comment
      comment: {
        required: true,
        type: Object,
      }
    },

    // Component methods
    methods: {
      // Load replies for specified comment
      loadReplies() {
        // comment.id is parent comment (base) for reply => comes from props
        return this.$axios.$get(`/api/comments/${this.comment.id}/replies`)
          // Assign replies that we got for specified comment to replies arr
          .then(replies => this.comments = replies)
      },
    },

    computed: {
      commentParentType() {
        return COMMENT_PARENT_TYPE.COMMENT
      }
    }

  }
</script>

<style scoped>

</style>
