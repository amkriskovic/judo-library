export const feed = (order) => ({
  props: {
    contentEndpoint: {
      required: false,
      type: String
    }
  },

  data: () => ({
    content: [],
    // Pagination
    cursor: 0,
    limit: 10,
    order: order,
    started: false,
    finished: false,
    loading: false,
  }),

  watch: {
    // Watch order -> data
    'order': function (newValue) {
      this.reloadContent()
    }
  },

  methods: {
    getContentUrl() {
      return `${this.contentEndpoint}${this.query}`
    },

    // Handles scrolling -> loading more submissions
    onScroll() {
      if (this.finished || this.loading) return;

      // Pagination, position of screen at the bottom => load more
      const loadMore = document.body.offsetHeight - (window.pageYOffset + window.innerHeight) < 500
      if (loadMore) {
        // Load another round of submissions
        this.loadContent()
      }
    },

    reloadContent() {
      this.content = []
      this.cursor = 0
      this.finished = false
      this.started = false
      return this.loadContent()
    },

    loadContent() {
      this.started = true
      this.loading = true

      return this.$axios.$get(this.getContentUrl())
        .then(content => {
          // Finished with loading
          this.finished = content.length < this.limit

          this.parseContent(content)

          // After we did 1st round, increment the cursor, we get next step of items
          this.cursor += content.length
        })
        .finally(() => this.loading = false)
    },

    parseContent(content) {
      content.forEach(x => {
        if (!this.content.some(y => y.id === x.id)){
          this.content.push(x)
        }
      })
    }
  },

  computed: {
    query() {
      return `?order=${this.order}&cursor=${this.cursor}&limit=${this.limit}`
    }
  }
})
