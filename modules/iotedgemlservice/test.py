import tensorflow as tf # подключаем TF
hello = tf.constant('Hello, TensorFlow!') # создаем объект из TF
sess = tf.InteractiveSession() # создаем сессию
print(sess.run(hello)) #сессия "выполняет" объект