#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Date    : 2015-09-13 15:33:42
# @Author  : n1k0 (nikobin222@gmail.com)
# @Link    : http://n1k0.top

import os
import sys 
import subprocess

allpath = []

def getfile(path) :
	for root ,dirname ,filename in os.walk(path):
		for f in filename:	
			print f
def  main():
	for path in allpath :
		getfile(path)
if __name__ == '__main__':
	global allpath 
	if  len(sys.argv) >1 :
		allpath.append(sys.argv[1]) 
	main()

