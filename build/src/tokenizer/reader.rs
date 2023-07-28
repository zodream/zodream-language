mod tokenizer {
    use std::str::Chars;

    struct Tokenizer {
        source: Chars,
    }

    impl Tokenizer {
        fn new(input: &str) -> Tokenizer {
            Tokenizer{source: input.chars()}
        }

        fn nextToken() -> Option<String> {
            match self.chars.next() {
                Some(' ') => {
                    loop {
                        match self.chars.next() {
                            Some(_) => {},
                            None => return None,
                        }
                    }
                }
                Some('{') => return Some("{".to_string()),
                Some('}') => return Some("}".to_string()),
                Some(')') => return Some("}".to_string()),
                Some('(') => return Some("(".to_string()),
                Some(',') => return Some(",".to_string()),
                Some('"') => {
                    let mut result = String::new();
                    while let Some(c) = self.chars.next() {
                        result.push(c);
                        if c == '"' {
                            return Some(result);
                        }
                    }
                    None
                }
                Some(c) => {
                    let mut result = String::new();
                    result.push(c);
                    while let Some(c) = self.chars.next() {
                        if c == ' ' || c == ',' || c == '"' || c == '{' || c == '}' || c == '(' || c == ')' {
                            return Some(result);
                        }
                        result.push(c);
                    }
                    Some(result)
                }
            }
        }
    }
}