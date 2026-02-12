import requests
import time

def main():
    # Start timing
    start_time = time.perf_counter()

    url = "https://gutendex.com/books/"
    authors = set()

    next_url = url
    pages = 0

    while next_url is not None and pages < 5:
        response = requests.get(next_url)
        response.raise_for_status()
        data = response.json()

        # Get books
        results = data.get("results", [])

        for book in results:
            authors_list = book.get("authors", [])
            for author in authors_list:
                name = author.get("name")
                if name and name.strip():
                    authors.add(name.strip())

        # Next page
        next_url = data.get("next")
        pages += 1

    # Write file
    file_path = "authors.txt"
    with open(file_path, "w", encoding="utf-8") as f:
        for author in authors:
            f.write(author + "\n")

    # Stop timing
    end_time = time.perf_counter()
    elapsed = end_time - start_time

    print(f"Completed! {len(authors)} authors were written in {file_path}")
    print(f"Elapsed time: {elapsed:.2f} seconds")

if __name__ == "__main__":
    main()
