export const feed = (order, waitAuth = false) => ({
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
    enabled: !waitAuth
  }),

  watch: {
    // Watch order -> data
    'order': function (newValue) {
      // Resetting
      this.content = []
      this.cursor = 0
      this.finished = false
      this.started = false
      this.loadContent()
    }
  },

  created() {
    return this.$store.dispatch('auth/_waitAuthenticated')
      .then(() => {
        this.enabled = true

        this.loadContent()
      })
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

    loadContent() {
      if (process.server) return;

      this.started = true
      this.loading = true

      if (!this.enabled) return ;

      return this.$axios.$get(this.getContentUrl())
        .then(content => {
          // Finished with loading
          this.finished = content.length < this.limit

          content.forEach(x => this.content.push(x))

          // After we did 1st round, increment the cursor, we get next step of items
          this.cursor += content.length
        })
        .finally(() => this.loading = false)
    }
  },

  computed: {
    query() {
      return `?order=${this.order}&cursor=${this.cursor}&limit=${this.limit}`
    }
  }
})
