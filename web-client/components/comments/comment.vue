<template>
  <div>
    <!-- 1# Binding from comment-body component :comment prop to our comment prop, emitting sendComment event -> both sending to same func -->
    <!-- Replaying to top comment, @load-replies event => loads replies -->
    <comment-body :comment="comment"
                  :parent-id="comment.id"
                  :parent-type="commentParentType"
                  @comment-created="(c) => content.push(c)"
                  @[loadRepliesEvent]="loadContent"/>

    <!-- 2# -->
    <div class="ml-5">
      <comment-body v-for="comment in content"
                    :comment="comment"
                    :parent-id="comment.id"
                    :parent-type="commentParentType"
                    @comment-created="(c) => content.push(c)"
                    :key="`reply-${comment.id}`"/>
    </div>

    <div class="d-flex justify-center" v-if="content.length > 0 && !finished">
      <v-btn outlined small @click="loadContent">Load More</v-btn>
    </div>
  </div>
</template>

<script>
  import CommentBody from "./comment-body";
  import {COMMENT_PARENT_TYPE, container} from "@/components/comments/_shared";
  import {feed} from "@/components/feed";

  export default {
    // Component name
    name: "comment",

    // Injected components
    components: {CommentBody},

    mixins: [feed('first')],

    // Component props => this is where comment info is stored
    props: {
      // Comment
      comment: {
        required: true,
        type: Object,
      }
    },

    methods: {
      getContentUrl() {
        return `/api/comments/${this.comment.id}/${this.commentParentType}${this.query}`
      }
    },

    computed: {
      commentParentType() {
        return COMMENT_PARENT_TYPE.COMMENT
      },

      loadRepliesEvent() {
        return this.content.length === 0 ? 'load-replies' : ''
      }
    }

  }
</script>

<style scoped>

</style>
