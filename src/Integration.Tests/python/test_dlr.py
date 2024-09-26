class HelloWorld:
    def __init__(self):
        pass

    def hello(self, name: str) -> str:
        print("Hello, " + name)
        return "Hello, " + name

def test_invocation():
    print("GETTING INSTANCE")
    return HelloWorld()
