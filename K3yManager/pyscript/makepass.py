import random, string

def GenPassword(length):

    Num = [random.choice(string.digits) for i in range(4)]
    lLetter = [random.choice(string.lowercase) for i in range(4)]
    bLetter = [random.choice(string.uppercase) for i in range(4)]
    asc     = [random.choice(string.punctuation)  for i in range(4)]

    myall =  Num+lLetter+bLetter+asc ; 
    random.shuffle(myall)
    genPwd = ''.join(myall) 
    return genPwd

if __name__ == '__main__':
    print GenPassword(16)
